using CodeRebirth.src.Content.Maps;
using HarmonyLib;
using UnityEngine;
using UsefulZapGun.Compatibility.CodeRebirth.Scripts;
using UsefulZapGun.Scripts.Hazards;

namespace UsefulZapGun.Compatibility.CodeRebirth.Patches
{
    internal class CodeRebirthMapHazardsPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(AirControlUnit), "Start")]
        static void TurretStartPatch(ref AirControlUnit __instance)
        {
            __instance.transform.Find("Body").gameObject.AddComponent<ACUShockableScript>();
        }
    }
}
