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
        internal static ConfigEntry<bool> enableItemCharging;

        internal static List<string> enemyList;
        internal static Dictionary<string, ConfigEntry<float>> multiplayerDict = new Dictionary<string, ConfigEntry<float>>();

        internal static ConfigEntry<float> chargeLifeTime;
        internal static ConfigEntry<bool> enableWeaponCharging;
        internal static ConfigEntry<float> needForShovelCharge;

        internal static ConfigEntry<bool> enableZapHazards;
        internal static ConfigEntry<int> SpiketrapZapNeeded;



        internal static void ConfigSetup()
        {
            cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "mborsh.UsefulZapGun.cfg"), true);

            enableLogging = cfg.Bind("General", "Enable logging", true);
            enemyListString = cfg.Bind("General", "Enemy list", "Red Locust Bees,Butler Bees,Docile Locust Bees"); //TODO: general -> enemies
            enemyList = enemyListString.Value.Split(',').ToList();

            enableItemCharging = cfg.Bind("Items", "Enable equipment charging", true, "Charging ratio: chargeMultiplier * (Time.deltaTime / item.itemProperties.batteryUsage)");

            enableWeaponCharging = cfg.Bind("Weapon", "Enable weapon charging", true, "x2 damage for charged weapon (shovel, stop sign, mace (code rebirth), etc.");
            chargeLifeTime = cfg.Bind("Weapon", "Time until charge runs out", 15f);
            needForShovelCharge = cfg.Bind("Weapon", "Zap gun charge% for charged state", 33f);

            enableZapHazards = cfg.Bind("Hazards", "Enable hazard zap", true);
            SpiketrapZapNeeded = cfg.Bind("Hazards", "Zap before deactivating the spiketrap", 4);


            CheckConfig();
        }

        private static void CheckConfig()
        {
            enemyList.Remove("Bruce");
            enemyList.Remove("Tornado");
        }

        internal static void CreateAndCheckConfigEntryForItem(string itemName, float multiplayer)
        {
            var configEntry = cfg.Bind("Items", itemName + ": Zap gun charge multiplayer", multiplayer);
            multiplayerDict.Add(itemName, configEntry);
        }
    }
}
