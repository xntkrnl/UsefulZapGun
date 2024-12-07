using BepInEx.Configuration;
using System.Drawing;

namespace UsefulZapGun
{
    internal class UZGConfig
    {
        internal static ConfigEntry<bool> enableLogging;
        internal static ConfigEntry<string> enemyList;
        internal static string[] enemyListArray;
        internal static ConfigEntry<bool> enableItemCharging;

        internal static void ConfigSetup(ConfigFile cfg)
        {
            enableLogging = cfg.Bind("General", "Enable logging", true);
            enemyList = cfg.Bind("General", "Enemy list", "Red Locust Bees,Butler Bees,Docile Locust Bees");
            enemyListArray = enemyList.Value.Split(',');
            enableItemCharging = cfg.Bind("General", "Enable item charging", true);
        }
    }
}
