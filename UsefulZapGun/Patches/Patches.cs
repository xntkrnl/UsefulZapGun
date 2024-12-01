using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UsefulZapGun.Patches
{
    internal class UZGPatches
    {
        [HarmonyPostfix, HarmonyPatch(typeof(MenuManager), "Start")]
        static void MenuManagerStartPatch()
        {
            var enemyArray = Resources.FindObjectsOfTypeAll<EnemyType>(); //thanks, Zaggy1024
            foreach (EnemyType enemy in enemyArray)
            {
                Plugin.SpamLog($"{enemy.enemyName} has been found!", Plugin.spamType.message);

                if (enemy.enemyName != "Red Locust Bees")
                    continue;
                else
                {
                    enemy.canBeStunned = true;
                    Plugin.SpamLog($"{enemy.enemyName} can be stunned now!", Plugin.spamType.info);
                }
            }
        }


    }
}
