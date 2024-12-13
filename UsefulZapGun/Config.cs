using BepInEx;
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
        internal static List<string> enemyList;
        internal static ConfigEntry<bool> enableItemCharging;
        internal static ConfigEntry<bool> enableWeaponCharging;
        internal static ConfigEntry<float> timeUntilCharge;
        internal static ConfigEntry<float> chargeLifeTime;
        internal static List<ConfigEntry<float>> multiplayerList = new List<ConfigEntry<float>>();
        internal static ConfigEntry<bool> despawnEnemy;


        internal static void ConfigSetup()
        {
            cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "mborsh.UsefulZapGun.cfg"), true);

            enableLogging = cfg.Bind("General", "Enable logging", true);
            enemyListString = cfg.Bind("General", "Enemy list", "Red Locust Bees,Butler Bees,Docile Locust Bees");
            enemyList = enemyListString.Value.Split(',').ToList();

            enableItemCharging = cfg.Bind("Equipment", "Enable equipment charging", true, "Charging ratio: item.batteryUsage/22");

            enableWeaponCharging = cfg.Bind("Weapon", "Enable weapon charging", true, "x2 damage for charged weapon (shovel, stop sign, mace (code rebirth), etc.");
            timeUntilCharge = cfg.Bind("Weapon", "Time until charge", 3f);
            chargeLifeTime = cfg.Bind("Weapon", "Time until charge runs out", 15f);

            despawnEnemy = cfg.Bind("Enemy", "Despawn enemy", true, "If enabled - destroy the enemy\nIf disabled - try to kill the enemy, and if the enemy cannot be killed - destroy it.");

            CheckConfig();
        }

        private static void CheckConfig()
        {
            enemyList.Remove("Bruce");
            enemyList.Remove("Tornado");
        }

        internal static float CreateAndCheckConfigEntry(string section, string key, float multiplayer)
        {
            var configEntry = cfg.Bind(section, key, multiplayer);
            multiplayerList.Add(configEntry);
            return configEntry.Value;
        }
    }
}
