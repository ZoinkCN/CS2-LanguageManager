using Colossal.UI.Binding;
using Game.SceneFlow;
using Game.UI;
using LanguageManagerLib;
using System;

namespace LanguageManager.Systems
{
    public class LanguageManageSystem : UISystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();

            try
            {
                LanguageEntryManager.LangCode = GameManager.instance.localizationManager.activeLocaleId;

                Plugin.Log.LogMessage(LanguageEntryManager.LangCode);
                var plugins = LanguageEntryManager.RegisteredPlugins.Values;
                foreach (var plugin in plugins)
                {
                    foreach (var entry in plugin.Entries)
                    {
                        string id = plugin.PluginID;
                        string? key = entry.Key;
                        if (string.IsNullOrEmpty(key)) continue;
                        AddUpdateBinding(new GetterValueBinding<string>(id, key, () => entry.Value));
                        Plugin.Log.LogMessage($"{id}.{key}: {entry.Value}");
                    }
                }
            }
            catch (Exception e)
            {
                Plugin.Log.LogError(e.Message);
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();

            LanguageEntryManager.LangCode = GameManager.instance.localizationManager.activeLocaleId;
        }
    }
}
