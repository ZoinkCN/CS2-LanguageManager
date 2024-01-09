using Colossal.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;

namespace LanguageManagerLib
{
    public static class LanguageEntryManager
    {
        public class LanguageData
        {
            public LanguageData(string pluginID, string dir, string defaultCode)
            {
                PluginID = pluginID;
                RootDir = dir;
                DefaultCode = defaultCode;
            }

            public string PluginID { get; }
            public string DefaultCode { get; }
            public string RootDir { get; }
            public string LangCode { get; private set; } = default!;
            public Dictionary<string, string> Entries { get; private set; }

            public void LoadLanguage(string langCode)
            {
                LangCode = langCode;
                string filePath = Path.Combine(RootDir, $"{PluginID}.{LangCode}.json");
                if (!File.Exists(filePath))
                {
                    string defaultFilePath = Path.Combine(RootDir, $"{PluginID}.{DefaultCode}.json");
                    if (!File.Exists(defaultFilePath))
                        throw new FileNotFoundException(defaultFilePath);
                    filePath = defaultFilePath;
                }

                string json = File.ReadAllText(filePath);
                var temp = JSON.Load(json);
                Entries = JSON.MakeInto<Dictionary<string, string>>(temp);
            }
        }

        private static Dictionary<string, LanguageData> registeredPlugins = new Dictionary<string, LanguageData>();
        public static ReadOnlyDictionary<string, LanguageData> RegisteredPlugins => new ReadOnlyDictionary<string, LanguageData>(registeredPlugins);
        private static string langCode = default!;

        public static string LangCode
        {
            get => langCode;
            set
            {
                if (langCode != value)
                {
                    langCode = value;
                    foreach (var plugin in registeredPlugins.Values)
                    {
                        plugin.LoadLanguage(langCode);
                    }
                }
            }
        }

        public static void Register(string pluginID, string defaultLangCode)
        {
            string callerFilePath = Assembly.GetCallingAssembly().Location;
            if (callerFilePath == null) { return; }
            string dirName = Path.GetDirectoryName(callerFilePath);
            string languageDir = Path.Combine(dirName, "Languages");
            if (!Directory.Exists(languageDir))
            {
                Directory.CreateDirectory(languageDir);
            }
            registeredPlugins.Add(pluginID, new LanguageData(pluginID, languageDir, defaultLangCode));
        }

        public static void Unregister(string pluginID)
        {
            if (registeredPlugins.ContainsKey(pluginID))
            {
                registeredPlugins.Remove(pluginID);
            }
        }

        public static bool CheckRegister(string pluginID)
        {
            return registeredPlugins.ContainsKey(pluginID);
        }
    }
}
