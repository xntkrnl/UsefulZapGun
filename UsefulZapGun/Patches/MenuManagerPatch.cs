using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace UsefulZapGun.Patches
{
    internal class MenuManagerPatch
    {

        [HarmonyPostfix, HarmonyPatch(typeof(MenuManager), "Start")]
        static void FindAllEnemiesPatch()
        {
            if (Plugin.enemiesAndItemsFound)
                return;

            var enemyArray = Resources.FindObjectsOfTypeAll<EnemyType>(); //thanks, Zaggy1024
            foreach (EnemyType enemy in enemyArray)
            {
                Plugin.SpamLog($"{enemy.enemyName} has been found!", Plugin.spamType.message);

                if (UZGConfig.enemyList.Contains(enemy.enemyName))
                {
                    UZGConfig.CreateAndCheckConfigEntryForEnemy(enemy, 3f);
                    enemy.canBeStunned = true;
                    enemy.canBeDestroyed = true;
                    Plugin.SpamLog($"{enemy.enemyName} can be stunned and destoyed now!", Plugin.spamType.info);
                }
            }

            Plugin.SpamLog("---------------------------------------------------------", Plugin.spamType.message);

            if (UZGConfig.enableItemCharging.Value)
            {
                var itemArray = Resources.FindObjectsOfTypeAll<Item>();
                foreach (Item item in itemArray)
                {
                    if (item.requiresBattery)
                    {
                        UZGConfig.CreateAndCheckConfigEntryForItem(item, item.batteryUsage / 22);
                        Plugin.SpamLog($"{item.itemName} has been found!", Plugin.spamType.message);
                    }
                }
            }

            Plugin.enemiesAndItemsFound = true; //exiting to the menu launches this too, so we prevent the search just in case
        }
    }
}
