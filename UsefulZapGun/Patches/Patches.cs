using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Patches
{
    internal class UZGPatches
    {
        static bool alreadyPatched = false;
        static GameObject netHandler;
        static NetStuff hostNetHandler;

        [HarmonyPostfix, HarmonyPatch(typeof(GameNetworkManager), "Start")]
        static void AddPrefabsToNetwork()
        {
            string sAssemblyLocation = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var mainAssetBundle = AssetBundle.LoadFromFile(Path.Combine(sAssemblyLocation, "zapgunnetworkobject"));

            netHandler = mainAssetBundle.LoadAsset<GameObject>("zapgunnetworkobject.prefab");

            netHandler.AddComponent<NetStuff>();
            NetworkManager.Singleton.AddNetworkPrefab(netHandler);
        }

        [HarmonyPrefix, HarmonyPatch(typeof(StartOfRound), "Start")]
        public static void SpawnNetworkHandler()
        {
            if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsServer)
            {
                var goNetHandler = StartOfRound.Instantiate(netHandler);
                goNetHandler.GetComponent<NetworkObject>().Spawn();
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(StartOfRound), "Start")]
        public static void FindNetworkHandler()
        {
            hostNetHandler = GameObject.FindAnyObjectByType<NetStuff>();
        }

        [HarmonyPostfix, HarmonyPatch(typeof(MenuManager), "Start")]
        static void MenuManagerStartPatch()
        {
            if (alreadyPatched)
                return;

            var enemyArray = Resources.FindObjectsOfTypeAll<EnemyType>(); //thanks, Zaggy1024
            foreach (EnemyType enemy in enemyArray)
            {
                Plugin.SpamLog($"{enemy.enemyName} has been found!", Plugin.spamType.message);

                if (UZGConfig.enemyListArray.Contains(enemy.enemyName))
                {
                    enemy.canBeStunned = true;
                    Plugin.SpamLog($"{enemy.enemyName} can be stunned now!", Plugin.spamType.info);
                }
            }

            alreadyPatched = true; //exiting to the menu launches this too, so we prevent the search just in case
        }

        [HarmonyPostfix, HarmonyPatch(typeof(EnemyAICollisionDetect), "IShockableWithGun.ShockWithGun")]
        static void ShockWithGunPatch(ref EnemyAI ___mainScript, ref EnemyAICollisionDetect __instance)
        {
            if (UZGConfig.enemyListArray.Contains(___mainScript.enemyType.enemyName))
            {
                StartOfRound.Instance.StartCoroutine(WaitAndDoSmth(___mainScript, __instance));
            }
        }

        private static IEnumerator WaitAndDoSmth(EnemyAI beeScript, EnemyAICollisionDetect instance)
        {
            yield return new WaitForSeconds(3f);
            
            if (beeScript.stunNormalizedTimer > 0f)
            {
                var allZapGuns = GameObject.FindObjectsOfType<PatcherTool>(); //uuh i don't feel comfortable to do it at runtime
                foreach (PatcherTool zapgun in allZapGuns)
                {
                    if (zapgun.isBeingUsed && zapgun.shockedTargetScript == instance && GameNetworkManager.Instance.localPlayerController == zapgun.playerHeldBy)
                    {
                        zapgun.StopShockingAnomalyOnClient(true);
                        NetworkObjectReference enemyNOR = new NetworkObjectReference(beeScript.gameObject.GetComponent<NetworkObject>());
                        hostNetHandler.DestroyEnemyServerRpc(enemyNOR); //i need a sanity check
                        break;
                    }
                }
            }
        }
    }
}
