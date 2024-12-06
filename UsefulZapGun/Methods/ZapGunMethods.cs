using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Methods
{
    internal class ZapGunMethods
    {
        internal static IEnumerator WaitAndDoSmth(EnemyAI enemyScript, EnemyAICollisionDetect instance)
        {
            yield return new WaitForSeconds(3f);

            if (enemyScript.stunNormalizedTimer > 0f)
            {
                var allZapGuns = GameObject.FindObjectsOfType<PatcherTool>(); //uuh i don't feel comfortable to do it at runtime
                foreach (PatcherTool zapgun in allZapGuns)
                {
                    if (zapgun.isBeingUsed && zapgun.shockedTargetScript == instance && GameNetworkManager.Instance.localPlayerController == zapgun.playerHeldBy)
                    {
                        zapgun.StopShockingAnomalyOnClient(true);
                        NetworkObjectReference enemyNOR = new NetworkObjectReference(enemyScript.gameObject.GetComponent<NetworkObject>());
                        GameNetworkManagerPatch.hostNetHandler.DestroyEnemyServerRpc(enemyNOR); //i need a sanity check
                        break;
                    }
                }
            }
        }
    }
}
