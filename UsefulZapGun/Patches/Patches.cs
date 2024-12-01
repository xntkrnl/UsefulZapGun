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
    internal class UZGPatches
    {
        static bool alreadyPatched = false;

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
                    if (zapgun.isBeingUsed && zapgun.shockedTargetScript == instance)
                    {
                        zapgun.StopShockingAnomalyOnClient(true);
                        break;
                    }
                }
                NetworkObject enemyNO = beeScript.gameObject.GetComponent<NetworkObject>();

                Landmine.SpawnExplosion(beeScript.gameObject.transform.position, true, 0, 3, 20, 3);
                DestroyEnemyServerRpc(enemyNO);
                //StartOfRound.Destroy(beeScript.gameObject);
            }
        }

        [ServerRpc]
        internal static void DestroyEnemyServerRpc(NetworkObject enemy)
        {
            enemy.Despawn();
        }
    }
}
