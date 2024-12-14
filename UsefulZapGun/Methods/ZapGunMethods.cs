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
                        tool.StopShockingAnomalyOnClient(true);
                        //do smth
                        GameNetworkManagerPatch.hostNetHandler.BlowUpEnemyServerRpc(enemyNOR);
                    }
                }
            }
        }

        //unused
        internal static IEnumerator DOTPlayer(GrabbableObject item, PlayerControllerB player, int index)
        {
            while (true)
            {
                yield return new WaitForSeconds(1f);
                if (player.ItemSlots[index] != item)
                    break;
                else
                    player.DamagePlayer(5, causeOfDeath: CauseOfDeath.Electrocution);
            }
        }
    }
}
