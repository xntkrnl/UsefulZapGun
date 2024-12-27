using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;

namespace UsefulZapGun.Patches
{
    internal class TerminalPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(Terminal), "Awake"), HarmonyPriority(Priority.Low)]
        static void AwakePatch(ref Terminal __instance)
        {
            foreach (Item item in __instance.buyableItemsList)
            {
                if (item.itemName == "Zap gun")
                {
                    item.creditsWorth = UZGConfig.zapgunPrice.Value;
                    Plugin.SpamLog("Zap gun price: " + item.creditsWorth, Plugin.spamType.message);
                    return;
                }
            }
        }
    }
}
