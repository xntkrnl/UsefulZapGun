﻿using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace UsefulZapGun
{
    internal class UZGConfig
    {
        private static ConfigFile cfg;

        internal static ConfigEntry<bool> enableLogging;
        internal static ConfigEntry<string> enemyListString;
        internal static ConfigEntry<bool> enableItemCharging;
        internal static Dictionary<Item, ConfigEntry<float>> multiplayerDict = new Dictionary<Item, ConfigEntry<float>>();

        internal static List<string> enemyList;
        internal static Dictionary<EnemyType, ConfigEntry<float>> timeDict = new Dictionary<EnemyType, ConfigEntry<float>>();
        internal static ConfigEntry<bool> enableExplosion;
        internal static ConfigEntry<bool> enableDOTEnemy;
        internal static ConfigEntry<bool> enableDOTPlayers;
        internal static ConfigEntry<int> zapDamage;
        internal static ConfigEntry<int> zapDamageToPlayer;
        internal static ConfigEntry<float> zapTimeToDamage;

        internal static ConfigEntry<float> chargeLifeTime;
        internal static ConfigEntry<bool> enableWeaponCharging;
        internal static ConfigEntry<float> needForShovelCharge;

        internal static ConfigEntry<bool> enableZapHazards;
        internal static ConfigEntry<float> timeNeedForTurretDisable;
        internal static ConfigEntry<int> spiketrapZapNeeded;

        internal static void ConfigSetup()
        {
            cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "mborsh.UsefulZapGun.cfg"), true);

            enableLogging = cfg.Bind("General", "Enable logging", true);

            enemyListString = cfg.Bind("Enemies", "Enemy list", "Red Locust Bees,Butler Bees,Docile Locust Bees");
            enableExplosion = cfg.Bind("Enemies", "Enable enemy explosion", true);
            enableDOTEnemy = cfg.Bind("Enemies", "Damage enemy with zap", false);
            enableDOTPlayers = cfg.Bind("Enemies", "Damage players with zap", false);
            zapDamage = cfg.Bind("Enemies", "Zapgun damage to enemy", 1);
            zapDamageToPlayer = cfg.Bind("Enemies", "Zapgun damage to player", 10);
            zapTimeToDamage = cfg.Bind("Enemies", "Time to damage (seconds)", 1f);

            enemyList = enemyListString.Value.Split(',').ToList();

            enableItemCharging = cfg.Bind("Items", "Enable equipment charging", true, "Charging ratio: chargeMultiplier * (Time.deltaTime / item.itemProperties.batteryUsage)");

            enableWeaponCharging = cfg.Bind("Weapon", "Enable weapon charging", true, "x2 damage for charged weapon (shovel, stop sign, mace (code rebirth), etc.");
            chargeLifeTime = cfg.Bind("Weapon", "Time until charge runs out", 15f);
            needForShovelCharge = cfg.Bind("Weapon", "Zap gun charge% for charged state", 33f);

            enableZapHazards = cfg.Bind("Hazards", "Enable hazard zap", true);
            timeNeedForTurretDisable = cfg.Bind("Hazards", "Time need for turret", 3f);
            spiketrapZapNeeded = cfg.Bind("Hazards", "Zaps before deactivating the spiketrap", 2);


            CheckConfig();
        }

        private static void CheckConfig()
        {
            enemyList.Remove("Bruce");
            enemyList.Remove("Tornado");

            if (zapDamage.Value <= 0)
                zapDamage.Value = 1;
        }

        internal static void CreateAndCheckConfigEntryForItem(Item item, float multiplayer)
        {
            var configEntry = cfg.Bind("Items", item.itemName + ": charge multiplayer", multiplayer);
            multiplayerDict.TryAdd(item, configEntry);
        }

        internal static void CreateAndCheckConfigEntryForEnemy(EnemyType enemy, float time)
        {
            var configEntry = cfg.Bind("Enemies", enemy.enemyName + ": time needed for explosion", time);
            if (configEntry.Value <= 0f)
                configEntry.Value = 1f;

            timeDict.TryAdd(enemy, configEntry);
        }
    }
}
