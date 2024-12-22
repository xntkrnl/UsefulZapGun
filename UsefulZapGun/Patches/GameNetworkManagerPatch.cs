using HarmonyLib;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Network;

namespace UsefulZapGun.Patches
{
    internal class GameNetworkManagerPatch
    {
        internal static GameObject netHandler;
        internal static NetStuff hostNetHandler;

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
            hostNetHandler = UnityEngine.Object.FindAnyObjectByType<NetStuff>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), "Start")]
        static void AddPrefabsToNetwork()
        {
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "zapgunnetworkobject"));

            netHandler = mainAssetBundle.LoadAsset<GameObject>("zapgunnetworkobject.prefab");
            netHandler.AddComponent<NetStuff>();
            NetworkManager.Singleton.AddNetworkPrefab(netHandler);
        }
    }
}
