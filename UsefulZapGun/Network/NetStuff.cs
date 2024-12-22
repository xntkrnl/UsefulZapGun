using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;
using UsefulZapGun.Patches;
using UsefulZapGun.Scripts.Hazards;
using UsefulZapGun.Scripts.Items;

namespace UsefulZapGun.Network
{
    internal class NetStuff : NetworkBehaviour
    {
        internal GameObject wig;
        internal bool wigSpawned;
        internal PatcherTool zapgun;

        [ServerRpc(RequireOwnership = false)]
        internal void BlowUpEnemyServerRpc(NetworkObjectReference enemyNORef)
        {
            if (enemyNORef.TryGet(out NetworkObject enemyNO))
            {
                if (!UZGConfig.enemyList.Contains(enemyNO.GetComponent<EnemyAI>().enemyType.enemyName))
                {
                    Plugin.SpamLog("The client is trying to send an enemy that is not in the config!!!", Plugin.spamType.error);
                    return;
                }

                if (!UZGConfig.enableExplosion.Value)
                    return;

                Vector3 enemyPos = enemyNO.gameObject.transform.position;
                enemyNO.gameObject.GetComponent<EnemyAI>().KillEnemyServerRpc(true);
                BlowUpEnemyClientRpc(enemyPos);
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
        internal void SyncShovelDamageServerRpc(NetworkObjectReference shovelNORef)
        {
            if (shovelNORef.TryGet(out NetworkObject shovelNO))
            {
                if (!UZGConfig.enableWeaponCharging.Value)
                {
                    Plugin.SpamLog("The client is trying to charge a weapon!!!", Plugin.spamType.error);
                    return;
                }

                var shovelScript = shovelNO.gameObject.GetComponent<WeaponShockableScript>();
                SyncShovelDamageClientRpc(shovelNORef, UZGConfig.chargeLifeTime.Value);
            }
            else
                Plugin.SpamLog("The client is trying to send a non-existent shovel!!!", Plugin.spamType.error);
        }

        [ClientRpc]
        internal void SyncShovelDamageClientRpc(NetworkObjectReference shovelNORef, float seconds)
        {
            shovelNORef.TryGet(out NetworkObject shovelNO);
            shovelNO.GetComponent<WeaponShockableScript>().SyncDamageOnLocalClient(seconds);
        }

        [ClientRpc]
        internal void SyncCanShockSpikeClientRpc(NetworkObjectReference obj, bool canShock)
        {
            obj.TryGet(out NetworkObject netObj);
            var compSpike = netObj.GetComponentInChildren<SpiketrapShockableScript>();
            compSpike.SyncCanShockOnLocalClient(canShock);
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncZapCountServerRpc(NetworkObjectReference obj, int zapCount)
        {
            bool disable = zapCount >= UZGConfig.spiketrapZapNeeded.Value;
            if (disable)
                SyncCanShockSpikeClientRpc(obj, false);

            SyncZapCountClientRpc(obj, zapCount);
        }

        [ClientRpc]
        internal void SyncZapCountClientRpc(NetworkObjectReference obj, int zapCount)
        {
            obj.TryGet(out NetworkObject netObj);
            var comp = netObj.GetComponentInChildren<SpiketrapShockableScript>();

            if (comp.zapCount > zapCount)
                SyncZapCountServerRpc(obj, comp.zapCount + 1);
            else
                comp.zapCount = zapCount;
        }

        [ServerRpc(RequireOwnership = false)]
        internal void DOTPlayerServerRpc(NetworkBehaviourReference playerNBRef, int playerWhoHit)
        {
            if (playerNBRef.TryGet(out PlayerControllerB targetedPlayer))
            {
                if (!UZGConfig.enableDOTPlayers.Value || UZGConfig.zapDamageToPlayer.Value == 0)
                    return;

                foreach (PatcherTool zapgun2 in ZapGunMethods.zapGuns)
                {
                    if (zapgun2.isBeingUsed && zapgun2.shockedTargetScript == targetedPlayer)
                        zapgun = zapgun2;
                }

                StartCoroutine(ZapGunMethods.DOTPlayer(targetedPlayer, playerWhoHit, UZGConfig.zapDamageToPlayer.Value, UZGConfig.zapTimeToDamage.Value, zapgun));
            }
            else
                Plugin.SpamLog($"Can't find targetedPlayer!!!", Plugin.spamType.error);

        }

        [ServerRpc(RequireOwnership = false)]
        internal void DOTEnemyServerRpc(NetworkBehaviourReference enemyNBRef, NetworkBehaviourReference playerWhoHitNBRef)
        {
            if (enemyNBRef.TryGet(out EnemyAI enemyAI))
            {
                if (playerWhoHitNBRef.TryGet(out PlayerControllerB playerWhoHit))
                {
                    if (!UZGConfig.enableDOTEnemy.Value)
                        return;

                    var shockableScript = enemyAI.GetComponentInChildren<EnemyAICollisionDetect>();
                    if (shockableScript == null)
                    {
                        Plugin.SpamLog($"Can't find EnemyAICollisionDetect!!!", Plugin.spamType.error);
                        return;
                    }

                    foreach (PatcherTool zapgun2 in ZapGunMethods.zapGuns)
                    {
                        if (zapgun2.isBeingUsed && zapgun2.shockedTargetScript == shockableScript)
                            zapgun = zapgun2;
                    }

                    StartCoroutine(ZapGunMethods.DOTEnemy(shockableScript, playerWhoHit, UZGConfig.zapDamage.Value, UZGConfig.zapTimeToDamage.Value, zapgun));
                }
                else
                    Plugin.SpamLog($"Can't find playerWhoHit!!!", Plugin.spamType.error);
            }
            else
                Plugin.SpamLog($"The client is trying to send a non-existent enemy!!!", Plugin.spamType.error);

        }
    }
}
