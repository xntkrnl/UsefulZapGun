using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Patches
{
    internal class PatcherToolPatch
    {
        //[HarmonyPostfix, HarmonyPatch(typeof(PatcherTool), "Start")]


        [HarmonyPostfix, HarmonyPatch(typeof(PatcherTool), "BeginShockingAnomalyOnClient")]
        static void AddZapGun(ref PatcherTool __instance)
        {
            ZapGunMethods.zapGuns.Add(__instance);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PatcherTool), "StopShockingAnomalyOnClient")]
        static void RemoveZapGun(ref PatcherTool __instance)
        {
            if (!ZapGunMethods.zapGuns.Remove(__instance))
            {
                Plugin.SpamLog($"Couldn't remove the zap gun! networkObjectID: {__instance.NetworkObjectId}", Plugin.spamType.error);
            }
        }

    }
}
