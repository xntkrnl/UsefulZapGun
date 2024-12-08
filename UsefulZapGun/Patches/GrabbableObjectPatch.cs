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
        static void StartPatch(ref GrabbableObject __instance)
        {
            if (__instance is Shovel && !__instance.gameObject.TryGetComponent<WeaponShockableScript>(out WeaponShockableScript weaponComponent))
            {
                __instance.gameObject.AddComponent<WeaponShockableScript>();
                return;
            }
            if (__instance.insertedBattery != null && !__instance.gameObject.TryGetComponent<EquipmentShockableScript>(out EquipmentShockableScript itemComponent))
            {
                __instance.gameObject.AddComponent<EquipmentShockableScript>();
                return;
            }

            //todo: separate method for checking items and switch-case here
        }
    }
}
