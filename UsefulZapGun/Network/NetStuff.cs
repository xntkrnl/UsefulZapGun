using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Patches;
using UsefulZapGun.Scripts.Hazards;
using UsefulZapGun.Scripts.Items;

namespace UsefulZapGun.Network
{
    internal class NetStuff : NetworkBehaviour
    {
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

        [ServerRpc(RequireOwnership = false)]
        internal void SyncCanShockServerRpc(NetworkObjectReference obj, bool canShock)
        {
            SyncCanShockClientRpc(obj, canShock);
        }

        [ClientRpc]
        internal void SyncCanShockClientRpc(NetworkObjectReference obj, bool canShock)
        {
            obj.TryGet(out NetworkObject netObj);
            if (netObj.gameObject.TryGetComponent<SpiketrapShockableScript>(out SpiketrapShockableScript compSpike))
            {
                compSpike.SyncCanShockOnLocalClient(canShock);
                return;
            }
            if (netObj.gameObject.TryGetComponent<TurretShockableScript>(out TurretShockableScript compTurret))
            {
                compTurret.SyncCanShockOnLocalClient(canShock);
                return;
            }
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncZapCountServerRpc(NetworkObjectReference obj, int zapCount)
        {
            bool disable = zapCount >= UZGConfig.SpiketrapZapNeeded.Value;
            if (disable)
                SyncCanShockClientRpc(obj, false);

            SyncZapCountClientRpc(obj, zapCount);
        }

        [ClientRpc]
        internal void SyncZapCountClientRpc(NetworkObjectReference obj, int zapCount)
        {
            obj.TryGet(out NetworkObject netObj);
            var comp = netObj.GetComponent<SpiketrapShockableScript>();

            if (comp.zapCount > zapCount)
                SyncZapCountServerRpc(obj, comp.zapCount+1);
            else
                comp.zapCount = zapCount;
                
        }
    }
}
