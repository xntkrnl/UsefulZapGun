using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun
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
                        networkObject.Despawn();
                        DestroyEnemyClientRpc(networkObject.gameObject.transform.position);
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
