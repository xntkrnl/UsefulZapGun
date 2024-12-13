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
        static bool enemiesAndItemsFound = false;

        [HarmonyPostfix, HarmonyPatch(typeof(MenuManager), "Start")]
        static void FindAllEnemiesPatch()
        {
            if (enemiesAndItemsFound)
                return;

            var enemyArray = Resources.FindObjectsOfTypeAll<EnemyType>(); //thanks, Zaggy1024
            foreach (EnemyType enemy in enemyArray)
            {
                Plugin.SpamLog($"{enemy.enemyName} has been found!", Plugin.spamType.message);

                if (UZGConfig.enemyList.Contains(enemy.enemyName))
                {
                    enemy.canBeStunned = true;
                    enemy.canBeDestroyed = true;
                    Plugin.SpamLog($"{enemy.enemyName} can be stunned and destoyed now!", Plugin.spamType.info);
                }
            }

            if (UZGConfig.enableItemCharging.Value)
            {
                Plugin.SpamLog("---------------------------------------------------------", Plugin.spamType.message);

                var itemArray = Resources.FindObjectsOfTypeAll<Item>();
                foreach (Item item in itemArray)
                {
                    if (item.requiresBattery)
                    {
                        UZGConfig.CreateAndCheckConfigEntryForItem(item.itemName, item.batteryUsage / 22);
                        Plugin.SpamLog($"{item.itemName} has been found!", Plugin.spamType.message);
                    }
                }
            }

            enemiesAndItemsFound = true; //exiting to the menu launches this too, so we prevent the search just in case
        }
    }
}
