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
        internal void DestroyEnemyServerRpc(NetworkObjectReference enemy)
        {
            if (enemy.TryGet(out NetworkObject enemyNO))
            {
                Vector3 enemyPos = enemyNO.gameObject.transform.position;
                enemyNO.Despawn();
                DestroyEnemyClientRpc(enemyPos);
                return;
            }
            else
                Plugin.SpamLog("The client is trying to send a non-existent enemy!!!", Plugin.spamType.error);
        }

        [ClientRpc]
        internal void DestroyEnemyClientRpc(Vector3 position)
        {
            Landmine.SpawnExplosion(position, true, 0, 3, 20, 3);
        }
    }
}
