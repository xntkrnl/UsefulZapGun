using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UsefulZapGun.Scripts.Items;

namespace UsefulZapGun.Patches.Items
{
    internal class ShovelPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(Shovel), "Start")]
        static void StartPatch(ref Shovel __instance)
        {
            if (!__instance.gameObject.TryGetComponent(out WeaponShockableScript weaponComponent))
            {
                __instance.gameObject.AddComponent<WeaponShockableScript>();
                return;
            }
        }
    }
}
