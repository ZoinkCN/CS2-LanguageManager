﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;

namespace LanguageManagerLib
{
    public class Manager
    {
        private class LanguageData
        {
            public LanguageData(string dir, string defaultCode)
            {
                RootDir = dir;
                DefaultCode = defaultCode;
            }

            public string DefaultCode { get; }
            public string RootDir { get; }
            public string LangCode { get; private set; } = default!;
            public ReadOnlyDictionary<string, string> Entries { get; private set; }

            public void LoadLanguage(string langCode)
            {
                LangCode = langCode;
                string filePath = Path.Combine(RootDir, $"{LangCode}.json");
                if (!File.Exists(filePath))
                {
                    string defaultFilePath = Path.Combine(RootDir, $"{DefaultCode}.json");
                    if (!File.Exists(defaultFilePath))
                        throw new FileNotFoundException(defaultFilePath);
                    filePath = defaultFilePath;
                }
                using FileStream languageStream = File.OpenRead(filePath);
                using StreamReader languageStreamReader = new StreamReader(languageStream);
                string languageContent = languageStreamReader.ReadToEnd();

                try
                {
                    JsonDocument doc = JsonDocument.Parse(languageContent);
                    Entries = new ReadOnlyDictionary<string, string>(doc.RootElement.Deserialize<Dictionary<string, string>>());
                }
                catch
                {
                    throw new Exception($"Failed in loading {filePath}!");
                }
            }
        }

        private static readonly object Lock = new object();

        private static readonly Dictionary<string, LanguageData> RegisteredPlugins = new Dictionary<string, LanguageData>();
        private static string langCode = default!;

        public static string LangCode
        {
            get => langCode;
            set
            {
                if (langCode != value)
                {
                    langCode = value;
                    foreach (var plugin in RegisteredPlugins.Values)
                    {
                        plugin.LoadLanguage(langCode);
                    }
                }
            }
        }
        public static void Register(string pluginID, string defaultLangCode, [CallerFilePath] string? callerFilePath = null)
        {
            if (callerFilePath == null) { return; }
            string dirName = Path.GetDirectoryName(callerFilePath);
            string languageDir = Path.Combine(dirName, "Languages");
            if (!Directory.Exists(languageDir))
            {
                Directory.CreateDirectory(languageDir);
            }
            lock (Lock)
            {
                RegisteredPlugins.Add(pluginID, new LanguageData(languageDir, defaultLangCode));
            }
        }

        public static void Unregister(string pluginID)
        {
            if (RegisteredPlugins.ContainsKey(pluginID))
            {
                lock (Lock)
                {
                    RegisteredPlugins.Remove(pluginID);
                }
            }
        }

        public static bool CheckRegister(string pluginID)
        {
            return RegisteredPlugins.ContainsKey(pluginID);
        }
    }
}