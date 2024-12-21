using HarmonyLib;
using UnityEngine;
using UsefulZapGun.Scripts.Items;

namespace UsefulZapGun.Patches.Items
{
    internal class GrabbableObjectPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GrabbableObject), "Start")]
        static void StartPatch(ref GrabbableObject __instance)
        {
            if (__instance is Shovel)
                return;


            if (__instance.itemProperties.requiresBattery && !__instance.gameObject.TryGetComponent(out EquipmentShockableScript itemComponent) && __instance.itemProperties.requiresBattery)
            {
                __instance.gameObject.AddComponent<EquipmentShockableScript>();
                return;
            }

            /*if (!__instance.gameObject.TryGetComponent(out ConductiveShockableScript conductiveItem) && __instance.itemProperties.isConductiveMetal)
            {
                __instance.gameObject.AddComponent<ConductiveShockableScript>();
                return;
            }*/
        }
    }
}
