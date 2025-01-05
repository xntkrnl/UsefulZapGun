using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Compatibility.CodeRebirth.Network;

namespace UsefulZapGun.Compatibility.CodeRebirth.Patches
{
    internal class CodeRebirthGameNetworkManagerPatch
    {
        internal static GameObject netHandler;
        internal static CodeRebirthNetwork rebirthNetwork;
        internal static GameObject hostNetHandler;

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
            rebirthNetwork = UnityEngine.Object.FindAnyObjectByType<CodeRebirthNetwork>();
            CRPlugin.SpamLog("rebirthNerwork found", CRPlugin.spamType.debug);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), "Start"), HarmonyAfter("mborsh.UsefulZapGun")]
        static void AddPrefabsToNetwork()
        {
            //CRUsefulZapGunNO
            netHandler = Plugin.mainAssetBundle.LoadAsset<GameObject>("CRUsefulZapGunNO.prefab");
            netHandler.AddComponent<CodeRebirthNetwork>();
            NetworkManager.Singleton.AddNetworkPrefab(netHandler);
        }
    }
}
