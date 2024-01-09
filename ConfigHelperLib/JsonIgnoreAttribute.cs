using System;

namespace ConfigHelperLib
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class JsonIgnoreAttribute : Attribute
    {
    }
}
