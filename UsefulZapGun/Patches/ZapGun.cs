using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Patches
{
    internal class ZapGun
    {
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
                        GameNetworkManagerPatch.hostNetHandler.DestroyEnemyServerRpc(enemyNOR); //i need a sanity check
                        break;
                    }
                }
            }
        }
    }
}
