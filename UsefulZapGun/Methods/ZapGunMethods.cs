using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Methods
{
    internal class ZapGunMethods
    {
        internal static List<PatcherTool> zapGuns = new List<PatcherTool>();
        internal static Coroutine explosionCoroutine;

        internal static IEnumerator ExplodeNextFrame(Vector3 position)
        {
            yield return new WaitForEndOfFrame();
            Landmine.SpawnExplosion(position, true, 0, 5, 20, 3);
        }

        internal static void WaitAndExplode(EnemyAICollisionDetect enemyCol, EnemyAI enemyScript, float time)
        {
            foreach (PatcherTool tool in zapGuns)
            {
                if (tool.isBeingUsed && tool.playerHeldBy == GameNetworkManager.Instance.localPlayerController && tool.shockedTargetScript == enemyCol)
                {
                    explosionCoroutine = StartOfRound.Instance.StartCoroutine(WaitAndExplodeCoroutine(tool, enemyCol, enemyScript, time));
                }
            }
        }

        internal static IEnumerator WaitAndExplodeCoroutine(PatcherTool zapgun, EnemyAICollisionDetect enemyCol, EnemyAI enemyScript, float time)
        {
            Plugin.SpamLog($"is zapgun null? {zapgun == null}", Plugin.spamType.debug);

            while (time > 0)
            {
                //yield return new WaitForEndOfFrame();
                //time -= Time.deltaTime;
                yield return new WaitForSeconds(0.1f);
                time -= 0.1f;
                Plugin.SpamLog($"time = {time}", Plugin.spamType.debug);

                if (!zapgun.isBeingUsed || zapgun.shockedTargetScript != enemyCol)
                {
                    Plugin.SpamLog("Stop charging explosion!", Plugin.spamType.debug);
                    StartOfRound.Instance.StopCoroutine(explosionCoroutine);
                }
            }

            zapgun.StopShockingAnomalyOnClient(true);
            yield return new WaitForEndOfFrame();
            var enemyNORef = new NetworkObjectReference(enemyScript.NetworkObject);
            GameNetworkManagerPatch.hostNetHandler.BlowUpEnemyServerRpc(enemyNORef);
        }

        internal static IEnumerator DOTPlayer(PlayerControllerB player, int playerWhoHit, int damageToPlayer, float time, PatcherTool zapgun)
        {
            while (zapgun.shockedTargetScript == player && zapgun.isBeingUsed)
            {
                player.DamagePlayerFromOtherClientServerRpc(damageToPlayer, player.transform.position, playerWhoHit);
                yield return new WaitForSeconds(time);
            }
        }

        internal static IEnumerator DOTEnemy(EnemyAICollisionDetect enemyAICol, PlayerControllerB playerWhoHit, int force, float time, PatcherTool zapgun)
        {
            var enemyAIColIHittable = (IHittable)enemyAICol;

            while (zapgun.shockedTargetScript == enemyAICol && zapgun.isBeingUsed)
            {
                enemyAIColIHittable.Hit(force, enemyAICol.transform.position, playerWhoHit);
                yield return new WaitForSeconds(time);
            }
        }
    }
}
