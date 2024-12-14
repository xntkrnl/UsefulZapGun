using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UsefulZapGun.Scripts.Hazards;

namespace UsefulZapGun.Patches
{
    internal class MapHazardsPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(Turret), "Start")]
        static void TurretStartPatch(ref Turret __instance)
        {
            __instance.gameObject.AddComponent<TurretShockableScript>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Landmine), "Start")]
        static void LandmineStartPatch(ref Landmine __instance)
        {
            __instance.gameObject.AddComponent<LandmineShockableScript>();
        }
    }
}
