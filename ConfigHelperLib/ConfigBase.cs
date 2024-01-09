using Colossal;
using Colossal.Json;
using Colossal.Logging;
using Colossal.OdinSerializer.Utilities;
using Colossal.Reflection;
using Colossal.UI.Binding;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Reflection;

namespace ConfigHelperLib
{
    public class ConfigBase : IJsonWritable, IJsonReadable
    {
        private static readonly HashSet<Type> m_PrimitiveTypes = new HashSet<Type>
        {
            typeof(float),
            typeof(double),
            typeof(int),
            typeof(uint),
            typeof(long),
            typeof(sbyte),
            typeof(byte),
            typeof(short),
            typeof(ushort),
            typeof(ulong),
            typeof(decimal),
        };

        public void Write(IJsonWriter writer)
        {
            WriteObject(writer, this);
        }

        private void WriteValue(IJsonWriter writer, object value)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else if (value is Level)
            {
                writer.Write(value.ToString());
            }
            else if (value is string stringValue)
            {
                writer.Write(stringValue);
            }
            else if (value is char)
            {
                writer.Write(value.ToString());
            }
            else if (value is bool boolValue)
            {
                writer.Write(boolValue);
            }
            else if (value is Enum)
            {
                writer.Write(value.ToString());
            }
            else if (value is Array array)
            {
                WriteArray(writer, array);
            }
            else if (value is IList list)
            {
                WriteList(writer, list);
            }
            else if (value is IDictionary dictionary)
            {
                WriteDictionary(writer, dictionary);
            }
            else if (value is Guid guid)
            {
                writer.Write(guid.ToLowerNoDashString());
            }
            else if (value is Type typeValue)
            {
                writer.Write(typeValue.FullName);
            }
            else if (value is Hash64)
            {
                writer.Write(value.ToString());
            }
            else if (value is Hash128)
            {
                writer.Write(value.ToString());
            }
            else if (value is IPAddress address)
            {
                writer.Write(address.ToString());
            }
            else if (value is DateTime time)
            {
                writer.Write(time.ToUniversalTime().ToString(CultureInfo.InvariantCulture));
            }
            else if (value is TimeSpan span)
            {
                writer.Write(span.ToString());
            }
            else if (value is Variant)
            {
                throw new NotSupportedException(typeof(Variant).FullName);
            }
            else if (m_PrimitiveTypes.Contains(value.GetType()))
            {
                writer.Write(Convert.ToString(value, CultureInfo.InvariantCulture));
            }
            else
            {
                WriteObject(writer, value);
            }
        }

        private void WriteObject(IJsonWriter writer, object value)
        {
            Type type = value.GetType();
            writer.TypeBegin(type.FullName);
            type.GetFields(BindingFlags.Instance | BindingFlags.Public).ForEach(field =>
            {
                if (!field.HasAttribute<JsonIgnoreAttribute>())
                {
                    writer.PropertyName(field.Name);
                    WriteValue(writer, field.GetValue(value));
                }
            });
            type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ForEach(property =>
            {
                if (!property.HasAttribute<JsonIgnoreAttribute>())
                {
                    ParameterInfo[] indexParameters = property.GetIndexParameters();
                    if (indexParameters.Length == 0)
                    {
                        writer.PropertyName(property.Name);
                        WriteValue(writer, property.GetValue(value));
                    }
                    else
                    {

                    }
                }
            });
            writer.TypeEnd();
        }

        private void WriteDictionary(IJsonWriter writer, IDictionary value)
        {
            if (value.Count == 0)
            {
                writer.WriteEmptyMap();
                return;
            }
            writer.MapBegin(value.Count);
            foreach (object key in value.Keys)
            {
                WriteValue(writer, key);
                WriteValue(writer, value[key]);
            }
            writer.MapEnd();
        }

        private void WriteList(IJsonWriter writer, IList value)
        {
            writer.ArrayBegin(value.Count);
            foreach (var item in value)
            {
                WriteValue(writer, item);
            }
            writer.ArrayEnd();
        }

        private void WriteArray(IJsonWriter writer, Array value)
        {
            if (value.Rank == 1)
            {
                WriteList(writer, value);
                return;
            }

            //int[] indices = new int[value.Rank];
            //WriteArrayRank(writer, value, 0, indices);
            throw new NotSupportedException("Array more than 1 dimension not supported"); // Maybe later, don't know how to read multi-dimension Array.
        }

        //private void WriteArrayRank(IJsonWriter writer, Array value, int rank, int[] indices)
        //{
        //    int lowerBound = value.GetLowerBound(rank);
        //    int upperBound = value.GetUpperBound(rank);
        //    writer.ArrayBegin(value.Length);
        //    if (rank == value.Rank - 1)
        //    {
        //        for (int i = lowerBound; i <= upperBound; i++)
        //        {
        //            indices[rank] = i;
        //            WriteValue(writer, value.GetValue(indices));
        //        }
        //    }
        //    else
        //    {
        //        for (int j = lowerBound; j <= upperBound; j++)
        //        {
        //            indices[rank] = j;
        //            WriteArrayRank(writer, value, rank + 1, indices);
        //        }
        //    }
        //    writer.ArrayEnd();
        //}

        public void Read(IJsonReader reader)
        {
            ReadObject(reader, this);
        }

        private object? ReadValue(IJsonReader reader, Type type)
        {
            if (type == typeof(Level))
            {
                reader.Read(out string str);
                return Level.GetLevel(str);
            }
            else if (type == typeof(string))
            {
                reader.Read(out string str);
                return str;
            }
            else if (type == typeof(char))
            {
                reader.Read(out string str);
                if (str.Length > 1) throw new Exception("Not a char!");
                return str[0];
            }
            else if (type == typeof(bool))
            {
                reader.Read(out bool boolValue);
                return boolValue;
            }
            else if (type.BaseType == typeof(Enum))
            {
                reader.Read(out string str);
                return Enum.Parse(type, str);
            }
            else if (type.BaseType == typeof(Array))
            {
                return ReadArray(reader, type);
            }
            else if (typeof(IList).IsAssignableFrom(type))
            {
                return ReadList(reader, type);
            }
            else if (typeof(IDictionary).IsAssignableFrom(type))
            {
                return ReadDictionary(reader, type);
            }
            else if (type == typeof(Guid))
            {
                reader.Read(out string str);
                return new Guid(str);
            }
            else if (type == typeof(Type))
            {
                reader.Read(out string str);
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                foreach (var assembly in assemblies)
                {
                    var value = assembly.GetType(str);
                    if (value != null)
                    {
                        return value;
                    }
                }
                throw new Exception($"Cannot find type {str}");
            }
            else if (type == typeof(Hash64))
            {
                reader.Read(out string str);
                return new Hash64(str);
            }
            else if (type == typeof(Hash128))
            {
                reader.Read(out string str);
                return new Hash128(str);
            }
            else if (type == typeof(IPAddress))
            {
                reader.Read(out string str);
                return IPAddress.Parse(str);
            }
            else if (type == typeof(DateTime))
            {
                reader.Read(out string str);
                return DateTime.Parse(str, CultureInfo.InvariantCulture);
            }
            else if (type == typeof(TimeSpan))
            {
                reader.Read(out string str);
                return TimeSpan.Parse(str);
            }
            else if (m_PrimitiveTypes.Contains(type.GetType()))
            {
                reader.Read(out string str);
                return Convert.ChangeType(str, type, CultureInfo.InvariantCulture);
            }
            else
            {
                return ReadObject(reader);
            }
        }

        private object? ReadObject(IJsonReader reader, object? obj = null)
        {
            reader.ReadMapBegin();
            Type type;
            if (reader.ReadProperty("__Type"))
            {
                type = (Type)ReadValue(reader, typeof(Type))!;
                if (obj != null && type != obj.GetType())
                {
                    throw new ArgumentException($"The \"obj\" type {obj.GetType()} not match {type}.");
                }
                else obj ??= Activator.CreateInstance(type);
            }
            else
            {
                if (obj == null)
                {
                    throw new Exception("No type declared in json, you need give an object to \"obj\"");
                }
                type = obj.GetType();
            }


            type.GetFields(BindingFlags.Instance | BindingFlags.Public).ForEach(field =>
            {
                if (!field.HasAttribute<JsonIgnoreAttribute>())
                {
                    if (reader.ReadProperty(field.Name))
                    {
                        field.SetValue(obj, ReadValue(reader, field.FieldType));
                    }
                }
            });
            type.GetProperties(BindingFlags.Instance | BindingFlags.Public).ForEach(property =>
            {
                if (!property.HasAttribute<JsonIgnoreAttribute>())
                {
                    ParameterInfo[] indexParameters = property.GetIndexParameters();
                    if (indexParameters.Length == 0)
                    {
                        if (reader.ReadProperty(property.Name))
                        {
                            property.SetValue(obj, ReadValue(reader, property.PropertyType));
                        }
                    }
                }
            });
            reader.ReadMapEnd();
            return obj;
        }

        private object ReadArray(IJsonReader reader, Type type)
        {
            Type elementType = type.GetElementType();
            Type listType = typeof(List<>).MakeGenericType(elementType);
            var toArray = listType.GetMethod("ToArray");
            IList list = (IList)Activator.CreateInstance(listType);

            uint count = reader.ReadArrayBegin();
            for (uint i = 0; i < count; i++)
            {
                reader.ReadArrayElement(i);
                var item = ReadValue(reader, elementType);
                list.Add(item);
            }
            reader.ReadArrayEnd();
            return toArray.Invoke(list, null);
        }

        private object ReadList(IJsonReader reader, Type type)
        {
            Type elementType = type.GetGenericArguments()[0];
            IList list = (IList)Activator.CreateInstance(type);

            uint count = reader.ReadArrayBegin();
            for (uint i = 0; i < count; i++)
            {
                reader.ReadArrayElement(i);
                var item = ReadValue(reader, elementType);
                list.Add(item);
            }
            reader.ReadArrayEnd();
            return list;
        }

        private object ReadDictionary(IJsonReader reader, Type type)
        {
            Type[] elementTypes = type.GetGenericArguments();
            Type keyType = elementTypes[0];
            Type valueType = elementTypes[1];
            IDictionary dict = (IDictionary)Activator.CreateInstance(type);

            uint count = reader.ReadMapBegin();
            for (int i = 0; i < count; i++)
            {
                reader.ReadMapKeyValue();
                var key = ReadValue(reader, keyType);
                var value = ReadValue(reader, valueType);
                dict.Add(key, value);
            }
            reader.ReadMapEnd();
            return dict;
        }
    }
}
