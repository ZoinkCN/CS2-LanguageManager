using Colossal.Json;
using Colossal.Logging;
using System;
using System.IO;

namespace ConfigHelperLib
{
    public static class ConfigHelper
    {
        public static T? LoadConfig<T>(string configPath, Action<Level, string>? callBack = null)
        {
            if (!File.Exists(configPath))
            {
                callBack?.Invoke(Level.Warn, $"Config File \"{configPath}\" Not Exist!");
                return default;
            }
            string json = File.ReadAllText(configPath);
            try
            {
                Variant variant = JSON.Load(json);
                return JSON.MakeInto<T>(variant);
            }
            catch (Exception e)
            {
                callBack?.Invoke(Level.Error, e.Message);
                return default;
            }
        }

        public static void SaveConfig<T>(string configPath, T config, Action<Level, string>? callBack = null)
        {
            string json = JSON.Dump(config);
            try
            {
                File.WriteAllText(configPath, json);
            }
            catch (Exception e)
            {
                callBack?.Invoke(Level.Error, e.Message);
            }
        }
    }
}
