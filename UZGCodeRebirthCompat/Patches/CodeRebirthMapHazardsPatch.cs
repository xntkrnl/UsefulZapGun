using CodeRebirth.src.Content.Maps;
using HarmonyLib;
using UnityEngine;
using UsefulZapGun.Compatibility.CodeRebirth.Scripts;

namespace UsefulZapGun.Compatibility.CodeRebirth.Patches
{
    internal class CodeRebirthMapHazardsPatch
    {
        //ACU my beloved
        [HarmonyPostfix, HarmonyPatch(typeof(AirControlUnit), "Start")]
        static void ACUStartPatch(ref AirControlUnit __instance)
        {
            if (CRConfig.enableACUZap.Value)
                __instance.transform.Find("Body").gameObject.AddComponent<ACUShockableScript>();
        }

        //BearTrap
        [HarmonyPostfix, HarmonyPatch(typeof(BearTrap), "Start")]
        static void BearTrapStartPatch(ref BearTrap __instance)
        {
            if (CRConfig.enableBearTrapZap.Value)
                __instance.gameObject.AddComponent<BearTrapShockableScript>();
        }

        //FlashTurret
        [HarmonyPostfix, HarmonyPatch(typeof(FlashTurret), "Start")]
        static void FlashStartPatch(ref FlashTurret __instance)
        {
            if (CRConfig.enableFlashZap.Value)
                __instance.gameObject.AddComponent<FlashShockableScript>();
        }

        //IndustrialFan
        [HarmonyPostfix, HarmonyPatch(typeof(IndustrialFan), "Start")]
        static void FanStartPatch(ref IndustrialFan __instance)
        {
            if (CRConfig.enableFanZap.Value)
                __instance.gameObject.AddComponent<IndustrialFanShockableScript>();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(IndustrialFan), "OnTriggerEnter")]
        static bool FanOnTriggerEnterPatch(ref IndustrialFan __instance)
        {
            return __instance.suctionForce != 0;
        }

        //LaserTurret
        [HarmonyPostfix, HarmonyPatch(typeof(LaserTurret), "Start")]
        static void LaserStartPatch(ref LaserTurret __instance)
        {
            if (CRConfig.enableLaserZap.Value)
                __instance.gameObject.AddComponent<LaserShockableScript>();
        }

        //Microwave
        [HarmonyPostfix, HarmonyPatch(typeof(FunctionalMicrowave), "Start")]
        static void MicrowaveStartPatch(ref FunctionalMicrowave __instance)
        {
            if (CRConfig.enableMicrowaveZap.Value)
                __instance.gameObject.AddComponent<MicrowaveShockableScript>();
        }

        //Tesla
        [HarmonyPostfix, HarmonyPatch(typeof(TeslaShock), "Start")]
        static void TeslaStartPatch(ref TeslaShock __instance)
        {
            if (CRConfig.enableTeslaZap.Value)
                __instance.gameObject.AddComponent<TeslaShockableScript>();
        }

        [HarmonyPrefix, HarmonyPatch(typeof(TeslaShock), "OnTriggerEnter")]
        static bool TeslaOnTriggerEnterPatch(ref TeslaShock __instance)
        {
            return __instance.enabled;
        }
    }
}
