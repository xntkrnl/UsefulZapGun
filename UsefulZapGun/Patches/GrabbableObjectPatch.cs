using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UsefulZapGun.Scripts;

namespace UsefulZapGun.Patches
{
    internal class GrabbableObjectPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GrabbableObject), "Start")]
        static void TestPatch(ref GrabbableObject __instance)
        {
            if (__instance.insertedBattery == null)
                return;

            if (!__instance.gameObject.TryGetComponent<ChargeEquipShockableScript>(out ChargeEquipShockableScript component))
                __instance.gameObject.AddComponent<ChargeEquipShockableScript>();
        }
    }
}
