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

        internal static IEnumerator WaitAndExplode(EnemyAI enemyScript, float time)
        {
            yield return new WaitForSeconds(time);

            if (enemyScript.stunNormalizedTimer > 0f)
            {
                foreach (PatcherTool tool in zapGuns)
                {
                    if (tool.isBeingUsed && tool.playerHeldBy == GameNetworkManager.Instance.localPlayerController)
                    {
                        NetworkObjectReference enemyNOR = new NetworkObjectReference(enemyScript.gameObject.GetComponent<NetworkObject>());
                        tool.StopShockingAnomalyOnClient(true);
                        //do smth
                        GameNetworkManagerPatch.hostNetHandler.BlowUpEnemyServerRpc(enemyNOR);
                    }
                }
            }
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
