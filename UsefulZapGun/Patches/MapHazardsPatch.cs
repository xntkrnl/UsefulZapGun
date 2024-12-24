using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UsefulZapGun.Scripts.Hazards;

namespace UsefulZapGun.Patches
{
    internal class MapHazardsPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(Turret), "Start")]
        static void TurretStartPatch(ref Turret __instance)
        {
            __instance.gameObject.AddComponent<TurretShockableScript>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(Landmine), "Start")]
        static void LandmineStartPatch(ref Landmine __instance)
        {
            __instance.gameObject.AddComponent<LandmineShockableScript>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(SpikeRoofTrap), "Start")]
        static void SpiketrapStartPatch(ref SpikeRoofTrap __instance)
        {
            var go = __instance.gameObject;
            go = go.transform.parent.Find("Spot Light").gameObject;

            var collider = go.AddComponent<BoxCollider>();
            //collider.size = new Vector3(0.1f, 0.1f, 0.1f);
            collider.isTrigger = true;

            go.AddComponent<SpiketrapShockableScript>();

            go.layer = 21;
            go.transform.parent.gameObject.layer = 21;
            go.transform.parent.parent.gameObject.layer = 21;
            go.transform.parent.parent.parent.gameObject.layer = 21;
        }
    }
}
