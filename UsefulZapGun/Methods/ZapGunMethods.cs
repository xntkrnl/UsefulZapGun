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

        internal static IEnumerator WaitAndDoSmth(EnemyAI enemyScript, EnemyAICollisionDetect instance)
        {
            yield return new WaitForSeconds(3f);

            if (enemyScript.stunNormalizedTimer > 0f)
            {
                foreach (PatcherTool tool in zapGuns)
                {
                    if (tool.isBeingUsed && tool.playerHeldBy == GameNetworkManager.Instance.localPlayerController)
                    {
                        NetworkObjectReference enemyNOR = new NetworkObjectReference(enemyScript.gameObject.GetComponent<NetworkObject>());
                        tool.StopShockingAnomalyOnClient();
                        //do smth
                        GameNetworkManagerPatch.hostNetHandler.DestroyEnemyServerRpc(enemyNOR);
                    }
                }
            }
        }
    }
}
