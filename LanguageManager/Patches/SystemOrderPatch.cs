using Game.Common;
using Game;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using LanguageManager.Systems;

namespace LanguageManager.Patches
{
    [HarmonyPatch(typeof(SystemOrder))]
    public static class SystemOrderPatch
    {
        [HarmonyPatch("Initialize")]
        [HarmonyPostfix]
        public static void Postfix(UpdateSystem updateSystem)
        {
            updateSystem.UpdateAt<LanguageManageSystem>(SystemUpdatePhase.UIUpdate);
        }
    }
}
