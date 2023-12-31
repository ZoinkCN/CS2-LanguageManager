﻿using Game;
using Game.Common;
using HarmonyLib;
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
