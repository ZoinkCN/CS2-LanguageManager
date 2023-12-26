using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Runtime.CompilerServices;

#if BEPINEX_V6
using BepInEx.Unity.Mono;
#endif

namespace LanguageManager
{
    [BepInPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_NAME, MyPluginInfo.PLUGIN_VERSION)]
    public class Plugin : BaseUnityPlugin
    {
        public static ManualLogSource Log { get; } = BepInEx.Logging.Logger.CreateLogSource(MyPluginInfo.PLUGIN_NAME);
        private void Awake()
        {
            var harmony = new Harmony(MyPluginInfo.PLUGIN_NAME);

            harmony.PatchAll();
            Logger.LogInfo($"Plugin {MyPluginInfo.PLUGIN_NAME} is loaded!");
        }

        public static void GetLanguageData(string langCode, [CallerFilePath] string? callerFilePath = null)
        {
            if (callerFilePath == null) { return; }
            string dirName = Path.GetDirectoryName(callerFilePath);
            string languageDir = Path.Combine(dirName, "Languages");
            if (!Directory.Exists(languageDir))
            {
                Directory.CreateDirectory(languageDir);
            }


        }
    }
}
