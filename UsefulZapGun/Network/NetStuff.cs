using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Patches;
using UsefulZapGun.Scripts.Hazards;
using UsefulZapGun.Scripts.Items;

namespace UsefulZapGun.Network
{
    internal class NetStuff : NetworkBehaviour
    {
        internal GameObject wig;
        internal bool wigSpawned;

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
        internal void WigServerRpc(NetworkObjectReference NORef)
        {
            wigSpawned = !wigSpawned;
            WigClientRpc(NORef, wigSpawned);
        }

        [ClientRpc]
        internal void WigClientRpc(NetworkObjectReference NORef, bool spawn)
        {
            NORef.TryGet(out NetworkObject NO);
            if (spawn)
            {
                wig = Instantiate(GameNetworkManagerPatch.wig, NO.transform.Find("ScavengerModel/metarig/spine/spine.001/spine.002/spine.003/spine.004/HatContainer"));
                wig.transform.localScale = new Vector3(1.7f, 1f, 1.7f);
            }
            else
                Destroy(wig);
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
            netObj.transform.Find("Container/AnimContainer/Trigger").gameObject.TryGetComponent<SpiketrapShockableScript>(out SpiketrapShockableScript compSpike);
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
            var comp = netObj.transform.Find("Container/AnimContainer/Trigger").gameObject.GetComponent<SpiketrapShockableScript>();

            if (comp.zapCount > zapCount)
                SyncZapCountServerRpc(obj, comp.zapCount + 1);
            else
                comp.zapCount = zapCount;

        }
    }
}
