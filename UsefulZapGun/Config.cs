using BepInEx;
using BepInEx.Configuration;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;

namespace UsefulZapGun
{
    public class UZGConfig
    {
        private static ConfigFile cfg;

        public static ConfigEntry<bool> enableLogging;
        public static ConfigEntry<int> zapgunPrice;
        public static ConfigEntry<bool> enableDifficultyMultiplierPatch;
        public static ConfigEntry<float> distanceDivider;

        public static ConfigEntry<string> enemyListString;
        public static Dictionary<Item, ConfigEntry<float>> multiplayerDict = new Dictionary<Item, ConfigEntry<float>>();

        public static List<string> enemyList;
        public static Dictionary<EnemyType, ConfigEntry<float>> timeDict = new Dictionary<EnemyType, ConfigEntry<float>>();
        public static ConfigEntry<bool> enableExplosion;
        public static ConfigEntry<bool> setForestGiantsOnFire;
        public static ConfigEntry<float> timeToStartAFire;
        public static ConfigEntry<bool> evaporateBlob;
        public static ConfigEntry<float> timeToEvaporate;

        public static ConfigEntry<bool> enableDOTEnemy;
        public static ConfigEntry<bool> enableDOTPlayers;
        public static ConfigEntry<int> zapDamage;
        public static ConfigEntry<int> zapDamageToPlayer;
        public static ConfigEntry<float> zapTimeToDamage;

        public static ConfigEntry<bool> enableItemCharging;

        public static ConfigEntry<float> chargeLifeTime;
        public static ConfigEntry<bool> enableWeaponCharging;
        public static ConfigEntry<float> needForShovelCharge;

        public static ConfigEntry<bool> enableZapHazards;
        public static ConfigEntry<float> timeNeedForTurretDisable;
        public static ConfigEntry<int> spiketrapZapNeeded;

        internal static void ConfigSetup(ConfigFile PluginConfig)
        {
            cfg = PluginConfig;

            enableLogging = cfg.Bind("General", "Enable logging", true);
            zapgunPrice = cfg.Bind("General", "Zap gun price", 400);
            enableDifficultyMultiplierPatch = cfg.Bind("General", "Enable Difficulty Multiplier Patch", true, "Formula: result = vanilaValue * Distance(localPlayer, enemy)/distanceDivider");
            distanceDivider = cfg.Bind("General", "Distance Divider", 5f);

            setForestGiantsOnFire = cfg.Bind("Enemies", "Set forest giants on fire", true);
            timeToStartAFire = cfg.Bind("Enemies", "Time to start a fire", 3f);
            evaporateBlob = cfg.Bind("Enemies", "Evaporate blob", true);
            enemyListString = cfg.Bind("Enemies", "Enemy list", "Red Locust Bees,Butler Bees");
            enableExplosion = cfg.Bind("Enemies", "Enable enemy explosion", true);

            enableDOTEnemy = cfg.Bind("DOT", "Damage enemy with zap", false);
            enableDOTPlayers = cfg.Bind("DOT", "Damage players with zap", false);
            zapDamage = cfg.Bind("DOT", "Zapgun damage to enemy", 1);
            zapDamageToPlayer = cfg.Bind("DOT", "Zapgun damage to player", 10);
            zapTimeToDamage = cfg.Bind("DOT", "Time to damage (seconds)", 1f);

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
