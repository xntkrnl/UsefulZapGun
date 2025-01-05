using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using System.IO;
using System.Reflection;
using UnityEngine;
using UsefulZapGun.Compatibility.CodeRebirth.Patches;
using UsefulZapGun;

[BepInPlugin(modGUID, modName, modVersion)]
[BepInDependency("mborsh.UsefulZapGun", BepInDependency.DependencyFlags.HardDependency)]
[BepInDependency("CodeRebirth", BepInDependency.DependencyFlags.SoftDependency)]
public class CRPlugin : BaseUnityPlugin
{
    //this crproj is a mess
    //most stuff is just a copy-paste from 0.4.0
    //maybe i will fix stuf here later

    //i apologize to anyone who reads this

    public static CRPlugin Instance { get; private set; }
    private static ManualLogSource mls;
    private readonly Harmony harmony = new Harmony(modGUID);
    private static ConfigFile cfg;

    private const string modGUID = "mborsh.UsefulZapGun.CRCompat";
    private const string modName = "UsefulZapGun.CRCompat";
    private const string modVersion = "0.0.1";

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

    private void Awake()
    {
        Instance = this;
        mls = BepInEx.Logging.Logger.CreateLogSource(modName);
        mls = Logger;

        if (!BepInEx.Bootstrap.Chainloader.PluginInfos.ContainsKey("CodeRebirth"))
        {
            mls.LogInfo("CodeRebirth is not installed, skipping!");
            return;
        }

        NetcodePatcher();

        cfg = new ConfigFile(Path.Combine(Paths.ConfigPath, $"{modGUID}.cfg"), true);
        CRConfig.RebirthConfigSetup(cfg);

        mls.LogInfo($"{modGUID} loaded. Patching.");
        harmony.PatchAll(typeof(CodeRebirthMapHazardsPatch));
        harmony.PatchAll(typeof(CodeRebirthGameNetworkManagerPatch));
    }

    internal static void SpamLog(string message, spamType type)
    {
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