using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UsefulZapGun.Patches
{
    internal class MenuManager
    {
        static bool alreadyPatched = false;

        [HarmonyPostfix, HarmonyPatch(typeof(MenuManager), "Start")]
        static void MenuManagerStartPatch()
        {
            if (alreadyPatched)
                return;

            var enemyArray = Resources.FindObjectsOfTypeAll<EnemyType>(); //thanks, Zaggy1024
            foreach (EnemyType enemy in enemyArray)
            {
                Plugin.SpamLog($"{enemy.enemyName} has been found!", Plugin.spamType.message);

                if (UZGConfig.enemyListArray.Contains(enemy.enemyName))
                {
                    enemy.canBeStunned = true;
                    Plugin.SpamLog($"{enemy.enemyName} can be stunned now!", Plugin.spamType.info);
                }
            }

            alreadyPatched = true; //exiting to the menu launches this too, so we prevent the search just in case
        }
    }
}
