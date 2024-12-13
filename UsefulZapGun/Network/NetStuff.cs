using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Network
{
    internal class NetStuff : NetworkBehaviour
    {

        [ServerRpc(RequireOwnership = false)]
        internal void BlowUpEnemyServerRpc(NetworkObjectReference enemyNORef)
        {
            if (enemyNORef.TryGet(out NetworkObject enemyNO))
            {
                Vector3 enemyPos = enemyNO.gameObject.transform.position;
                if (UZGConfig.despawnEnemy.Value)
                    enemyNO.Despawn();
                else
                {
                    enemyNO.gameObject.GetComponent<EnemyAI>().KillEnemyServerRpc(true);
                }

                BlowUpEnemyClientRpc(enemyPos);
                return;
            }
            else
                Plugin.SpamLog("The client is trying to send a non-existent enemy!!!", Plugin.spamType.error);
        }

        [ClientRpc]
        internal void BlowUpEnemyClientRpc(Vector3 position)
        {
            Landmine.SpawnExplosion(position, true, 0, 5, 20, 3);
        }

        [ServerRpc(RequireOwnership = false)]
        internal void HappyBirthdayRatServerRpc(NetworkObjectReference ratNORef)
        {
            HappyBirthdayRatClientRpc(ratNORef);
        }

        [ClientRpc]
        internal void HappyBirthdayRatClientRpc(NetworkObjectReference ratNORef)
        {
            ratNORef.TryGet(out NetworkObject ratNO);
            if (ratNO.GetComponent<PlayerControllerB>() == GameNetworkManager.Instance.localPlayerController)
                HUDManager.Instance.DisplayTip("HAPPY BIRTHDAY RAT", "HAPPY BIRTHDAY RAT\nHAPPY BIRTHDAY RAT\nHAPPY BIRTHDAY RAT\nHAPPY BIRTHDAY RAT\nHAPPY BIRTHDAY RAT\nHAPPY BIRTHDAY RAT");
        }
    }
}
