using HarmonyLib;
using UnityEngine;
using UsefulZapGun.Scripts;

namespace UsefulZapGun.Patches
{
    internal class GrabbableObjectPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GrabbableObject), "Start")]
        static void StartPatch(ref GrabbableObject __instance)
        {
            if (__instance is Shovel && !__instance.gameObject.TryGetComponent<WeaponShockableScript>(out WeaponShockableScript weaponComponent) && UZGConfig.enableWeaponCharging.Value)
            {
                __instance.gameObject.AddComponent<WeaponShockableScript>();
                return;
            }

            if (__instance.itemProperties.requiresBattery && !__instance.gameObject.TryGetComponent<EquipmentShockableScript>(out EquipmentShockableScript itemComponent) && UZGConfig.enableItemCharging.Value && __instance.itemProperties.requiresBattery)
            {
                __instance.gameObject.AddComponent<EquipmentShockableScript>();
                return;
            }

            if (UZGConfig.enableItemCharging.Value && !__instance.gameObject.TryGetComponent<ConductiveShockableScript>(out ConductiveShockableScript conductiveItem) && __instance.itemProperties.isConductiveMetal)
            {
                __instance.gameObject.AddComponent<ConductiveShockableScript>();
                return;
            }
        }
    }
}
