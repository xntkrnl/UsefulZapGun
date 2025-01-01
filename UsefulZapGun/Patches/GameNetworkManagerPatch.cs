using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Compatibility.CodeRebirth.Network;
using UsefulZapGun.Network;

namespace UsefulZapGun.Patches
{
    internal class GameNetworkManagerPatch
    {
        internal static GameObject netHandler;
        internal static UZGNetwork hostNetHandler;
        internal static CodeRebirthNetwork rebirthNetwork;

        [HarmonyPrefix, HarmonyPatch(typeof(StartOfRound), "Start")]
        public static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var goNetHandler = UnityEngine.Object.Instantiate(netHandler);
                goNetHandler.GetComponent<NetworkObject>().Spawn();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "Start")]
        public static void FindNetworkHandler()
        {
            hostNetHandler = UnityEngine.Object.FindAnyObjectByType<UZGNetwork>();
            if (Plugin.CREnabled) rebirthNetwork = hostNetHandler.GetComponent<CodeRebirthNetwork>();
            Plugin.SpamLog("hostNetHandler found", Plugin.spamType.debug);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), "Start")]
        static void AddPrefabsToNetwork()
        {
            netHandler = Plugin.mainAssetBundle.LoadAsset<GameObject>("zapgunnetworkobject.prefab");
            netHandler.AddComponent<UZGNetwork>();
            if (Plugin.CREnabled) netHandler.AddComponent<CodeRebirthNetwork>();
            NetworkManager.Singleton.AddNetworkPrefab(netHandler);
        }
    }
}
