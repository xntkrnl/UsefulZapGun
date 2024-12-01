using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using UsefulZapGun.Patches;

namespace UsefulZapGun
{
    [BepInDependency("ainavt.lc.lethalconfig", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(modGUID, modName, modVersion)]
    public class Plugin : BaseUnityPlugin
    {
        // Mod Details
        private const string modGUID = "mborsh.UsefulZapGun";
        private const string modName = "UsefulZapGun";
        private const string modVersion = "0.0.0";

        private readonly Harmony harmony = new Harmony(modGUID);
        private static ManualLogSource mls;
        private static Plugin Instance;

        void Awake()
        {
            Instance = this;
            mls = BepInEx.Logging.Logger.CreateLogSource(modName);
            mls = Logger;

            var cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, "mborsh.CruiserTerminal.cfg"), true);
            UZGConfig.ConfigSetup(cfg);

            mls.LogInfo($"{modName} {modVersion} loaded. Patching.");
            harmony.PatchAll(typeof(UZGPatches));
        }

        internal static void SpamLog(string message, spamType type)
        {
            //logging config

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