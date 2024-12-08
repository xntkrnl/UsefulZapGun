using BepInEx.Configuration;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace UsefulZapGun
{
    internal class UZGConfig
    {
        internal static ConfigEntry<bool> enableLogging;
        internal static ConfigEntry<string> enemyListString;
        internal static List<string> enemyList;
        internal static ConfigEntry<bool> enableItemCharging;
        internal static ConfigEntry<bool> enableWeaponCharging;
        internal static ConfigEntry<float> timeUntilCharge;
        internal static ConfigEntry<float> chargeLifeTime;


        internal static void ConfigSetup(ConfigFile cfg)
        {
            enableLogging = cfg.Bind("General", "Enable logging", true);
            enemyListString = cfg.Bind("General", "Enemy list", "Red Locust Bees,Butler Bees,Docile Locust Bees");
            enemyList = enemyListString.Value.Split(',').ToList();

            enableItemCharging = cfg.Bind("Equipment", "Enable equipment charging", true, "Charging ratio: item.batteryUsage/22");
            enableWeaponCharging = cfg.Bind("Weapon", "Enable weapon charging", true, "x2 damage for charged weapon (shovel, stop sign, mace (code rebirth), etc.");
            timeUntilCharge = cfg.Bind("Weapon", "Time until charge", 3f);
            chargeLifeTime = cfg.Bind("Weapon", "Time until charge runs out", 15f);

            CheckConfigForBruce();
        }

        private static void CheckConfigForBruce()
        {
            enemyList.Remove("Bruce");
            enemyList.Remove("Tornado");
        }
    }
}
