using CodeRebirth.src.Content.Maps;
using HarmonyLib;
using UnityEngine;
using UsefulZapGun.Compatibility.CodeRebirth.Scripts;
using UsefulZapGun.Scripts.Hazards;

namespace UsefulZapGun.Compatibility.CodeRebirth.Patches
{
    internal class CodeRebirthMapHazardsPatch
    {
        //ACU my beloved
        [HarmonyPostfix, HarmonyPatch(typeof(AirControlUnit), "Start")]
        static void ACUStartPatch(ref AirControlUnit __instance)
        {
            __instance.transform.Find("Body").gameObject.AddComponent<ACUShockableScript>();
        }

        //BearTrap
        [HarmonyPostfix, HarmonyPatch(typeof(BearTrap), "Start")]
        static void BearTrapStartPatch(ref BearTrap __instance)
        {
            __instance.gameObject.AddComponent<BearTrapShockableScript>();
        }

        //FlashTurret
        [HarmonyPostfix, HarmonyPatch(typeof(FlashTurret), "Start")]
        static void FlashStartPatch(ref FlashTurret __instance)
        {
            __instance.gameObject.AddComponent<FlashShockableScript>();
        }

        //IndustrialFan
        [HarmonyPostfix, HarmonyPatch(typeof(IndustrialFan), "Start")]
        static void FanStartPatch(ref IndustrialFan __instance)
        {
            __instance.gameObject.AddComponent<IndustrialFanShockableScript>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(IndustrialFan), "OnTriggerEnter")]
        static bool FanOnTriggerEnterPatch(ref IndustrialFan __instance)
        {
            return __instance.suctionForce != 0;
        }
    }
}
