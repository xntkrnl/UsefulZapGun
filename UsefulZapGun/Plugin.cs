using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace CruiserTerminal
{
    [BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(modGUID, modName, modVersion)]
    public class CTPlugin : BaseUnityPlugin
    {
        // Mod Details
        private const string modGUID = "UsefulZapGun";
        private const string modName = "UsefulZapGun";
        private const string modVersion = "1.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);
        public static ManualLogSource mls;
        private static CTPlugin Instance;

        void Awake()
        {
            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource("UsefulZapGun");
            mls = Logger;

            //var cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "mborsh.CruiserTerminal.cfg"), true);
            //CTConfig.Config(cfg);

            //if (BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("ainavt.lc.lethalconfig"))
            //    LethalConfigCompat.LethalConfigSetup();

            mls.LogInfo("UsefulZapGun loaded. Patching.");
            harmony.PatchAll();
        }
    }
}