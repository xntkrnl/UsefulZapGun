using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using UsefulZapGun.Patches;

namespace UsefulZapGun
{
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        // Mod Details
        private const string modGUID = "mborsh.UsefulZapGun";
        private const string modName = "UsefulZapGun";
        private const string modVersion = "0.1.0";

        private readonly Harmony harmony = new Harmony(modGUID);
        private static ManualLogSource mls;
        private static Plugin Instance;

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

            var cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, $"{modGUID}.cfg"), true);
            UZGConfig.ConfigSetup(cfg);

            mls.LogInfo($"{modName} {modVersion} loaded. Patching.");
            harmony.PatchAll(typeof(EnemyAICollisionDetectPatch));
            harmony.PatchAll(typeof(GameNetworkManagerPatch));
            harmony.PatchAll(typeof(MenuManagerPatch));
            harmony.PatchAll(typeof(PatcherToolPatch));

            if (UZGConfig.enableItemCharging.Value)
                harmony.PatchAll(typeof(GrabbableObjectPatch));
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
    }
}