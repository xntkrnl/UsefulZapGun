using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UsefulZapGun.Scripts;

namespace UsefulZapGun.Patches
{
    internal class PatchForTestItemShockableScript
    {
        [HarmonyPostfix, HarmonyPatch(typeof(FlashlightItem), "Start")]
        static void TestPatch(ref FlashlightItem __instance)
        {
            if (!__instance.gameObject.TryGetComponent<TestEquipShockableScript>(out TestEquipShockableScript component))
                __instance.gameObject.AddComponent<TestEquipShockableScript>();
        }
    }
}
