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
        private static Coroutine coroutine;

        //explosion
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
                    coroutine = StartOfRound.Instance.StartCoroutine(WaitAndExplodeCoroutine(tool, enemyCol, enemyScript, time));
                }
            }
        }

        private static IEnumerator WaitAndExplodeCoroutine(PatcherTool zapgun, EnemyAICollisionDetect enemyCol, EnemyAI enemyScript, float time)
        {
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
                    StartOfRound.Instance.StopCoroutine(coroutine);
                }
            }

            zapgun.StopShockingAnomalyOnClient(true);
            yield return new WaitForEndOfFrame();
            var enemyNORef = new NetworkObjectReference(enemyScript.NetworkObject);
            GameNetworkManagerPatch.hostNetHandler.BlowUpEnemyServerRpc(enemyNORef);
        }

        //fire
        internal static void StartFire(EnemyAICollisionDetect enemyCol, ForestGiantAI enemyScript)
        {
            foreach (PatcherTool tool in zapGuns)
            {
                if (tool.isBeingUsed && tool.playerHeldBy == GameNetworkManager.Instance.localPlayerController && tool.shockedTargetScript == enemyCol)
                {
                    coroutine = StartOfRound.Instance.StartCoroutine(WaitAndStartFire(enemyScript, tool, enemyCol));
                }
            }
        }

        private static IEnumerator WaitAndStartFire(ForestGiantAI enemyScript, PatcherTool zapgun, EnemyAICollisionDetect enemyCol)
        {
            float time = UZGConfig.timeToStartAFire.Value;
            while (time > 0)
            {
                yield return new WaitForSeconds(0.1f);
                time -= 0.1f;
                Plugin.SpamLog($"time = {time}", Plugin.spamType.debug);

                if (!zapgun.isBeingUsed || zapgun.shockedTargetScript != enemyCol)
                {
                    Plugin.SpamLog("Stop zapping giant!", Plugin.spamType.debug);
                    StartOfRound.Instance.StopCoroutine(coroutine);
                }
            }

            yield return new WaitForEndOfFrame();
            enemyScript.HitFromExplosion(Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, enemyScript.transform.position));
        }
    }
}
