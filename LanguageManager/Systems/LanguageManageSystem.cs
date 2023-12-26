using Colossal.UI.Binding;
using Game.SceneFlow;
using Game.UI;
using LanguageManagerLib;

namespace LanguageManager.Systems
{
    public class LanguageManageSystem : UISystemBase
    {
        protected override void OnCreate()
        {
            base.OnCreate();
            LanguageEntryManager.LangCode = GameManager.instance.localizationManager.activeLocaleId;

            var plugins = LanguageEntryManager.RegisteredPlugins.Values;
            foreach (var plugin in plugins)
            {
                string id = plugin.PluginID;
                foreach (var key in plugin.Entries.Keys)
                {
                    AddUpdateBinding(new GetterValueBinding<string>(id, key, () => plugin.Entries[key]));
                }
                Plugin.Log.LogMessage($"Plugin {id} registered");
            }
        }

        protected override void OnUpdate()
        {
            base.OnUpdate();
            LanguageEntryManager.LangCode = GameManager.instance.localizationManager.activeLocaleId;
        }
    }
}
