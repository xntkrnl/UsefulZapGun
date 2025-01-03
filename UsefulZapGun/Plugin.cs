﻿using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using UsefulZapGun.Compatibility.CodeRebirth.Patches;
using UsefulZapGun.Patches;
using UsefulZapGun.Patches.Enemy;
using UsefulZapGun.Patches.Items;

namespace UsefulZapGun
{
    [BepInDependency("TestAccount666.ShipWindows", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        // Mod Details
        private const string modGUID = "mborsh.UsefulZapGun";
        private const string modName = "UsefulZapGun";
        private const string modVersion = "0.4.0";

        private readonly Harmony harmony = new Harmony(modGUID);
        private static ManualLogSource mls;
        private static Plugin Instance;

        internal static bool enemiesAndItemsFound = false;

        private static string sAssemblyLocation;
        internal static AssetBundle mainAssetBundle;

        internal static bool CREnabled;

        private static void NetcodePatcher()
        {
            var types = Assembly.GetExecutingAssembly().GetTypes();
            foreach (var type in types)
            {
                var methods = type.GetMethods(BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
                foreach (var method in methods)
                {
                    var attributes = method.GetCustomAttributes(typeof(RuntimeInitializeOnLoadMethodAttribute), false);
                    if (attributes.Length > 0)
                    {
                        method.Invoke(null, null);
                    }
                }
            }
        }

        void Awake()
        {
            NetcodePatcher();

            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource(modName);
            mls = Logger;

            sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "zapgunnetworkobject"));

            CREnabled = BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("CodeRebirth");

            UZGConfig.ConfigSetup();

            mls.LogInfo($"{modName} {modVersion} loaded. Patching.");
            PatchAllStuff();
        }

        internal static void SpamLog(string message, spamType type)
        {
            if (!UZGConfig.enableLogging.Value)
                return;

            switch (type)
            {
                case spamType.info:
                    mls.LogInfo(message); break;
                case spamType.message:
                    mls.LogMessage(message); break;
                case spamType.warning:
                    mls.LogWarning(message); break;
                case spamType.debug:
                    mls.LogDebug(message); break;
                case spamType.error:
                    mls.LogError(message); break;
                case spamType.fatal:
                    mls.LogFatal(message); break;
                default: return;
            }
        }

        internal enum spamType
        {
            info,
            message,
            warning,
            debug,
            error,
            fatal
        }

        private void PatchAllStuff()
        {

            harmony.PatchAll(typeof(GameNetworkManagerPatch));
            harmony.PatchAll(typeof(MenuManagerPatch));

            harmony.PatchAll(typeof(PlayerControllerBPatch));

            harmony.PatchAll(typeof(TerminalPatch));

            harmony.PatchAll(typeof(EnemyAICollisionDetectPatch));
            harmony.PatchAll(typeof(EnemyAIPatch));

            harmony.PatchAll(typeof(PatcherToolPatch));

            if (UZGConfig.enableZapHazards.Value)
            {
                harmony.PatchAll(typeof(MapHazardsPatch));
                if (CREnabled)
                    harmony.PatchAll(typeof(CodeRebirthMapHazardsPatch));
            }

            if (UZGConfig.enableWeaponCharging.Value)
                harmony.PatchAll(typeof(ShovelPatch));

            if (UZGConfig.enableItemCharging.Value)
                harmony.PatchAll(typeof(GrabbableObjectPatch));

        }
    }
}