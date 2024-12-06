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
            var enemyID = enemy.NetworkObjectId;
            var enemyArray = FindObjectsOfType<EnemyAI>();
            if (enemyArray != null)
                foreach (EnemyAI enemyAI in enemyArray)
                {
                    var networkObject = enemyAI.gameObject.GetComponent<NetworkObject>();
                    var networkObjectID = networkObject.NetworkObjectId;
                    if (enemyID == networkObjectID)
                    {
                        Vector3 enemyPos = networkObject.gameObject.transform.position;
                        networkObject.Despawn();
                        DestroyEnemyClientRpc(enemyPos);
                        return;
                    }
                }
        }

        [ClientRpc]
        internal void DestroyEnemyClientRpc(Vector3 position)
        {
            Landmine.SpawnExplosion(position, true, 0, 3, 20, 3);
        }
    }
}
