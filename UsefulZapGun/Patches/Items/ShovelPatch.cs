using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UsefulZapGun.Scripts.Items;

namespace UsefulZapGun.Patches.Items
{
    internal class ShovelPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(GrabbableObject), "Start")]
        static void StartPatch(ref GrabbableObject __instance)
        {
            if (__instance is Shovel && !__instance.gameObject.TryGetComponent(out WeaponShockableScript weaponComponent))
            {
                __instance.gameObject.AddComponent<WeaponShockableScript>();
            }
        }
    }
}
