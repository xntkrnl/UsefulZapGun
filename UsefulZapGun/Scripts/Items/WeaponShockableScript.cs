using GameNetcodeStuff;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Scripts.Items
{
    internal class WeaponShockableScript : MonoBehaviour, IShockableWithGun
    {
        private Shovel itemScript;
        private bool charged;
        private Coroutine chargeCoroutine;
        private float batteryChargeNeedUntilChargedState;

        private void Start()
        {
            itemScript = GetComponent<Shovel>();
            charged = false;
            batteryChargeNeedUntilChargedState = UZGConfig.needForShovelCharge.Value / 100;
        }

        public bool CanBeShocked()
        {
            return itemScript.playerHeldBy == null && batteryChargeNeedUntilChargedState > 1;
        }

        public float GetDifficultyMultiplier()
        {
            return 0.3f;
        }

        public NetworkObject GetNetworkObject()
        {
            return itemScript.NetworkObject;
        }

        public Vector3 GetShockablePosition()
        {
            var position = base.transform.position;
            position = new Vector3(position.x, position.y + 0.2f, position.z);

            return position;
        }

        public Transform GetShockableTransform()
        {
            return transform;
        }

        public void ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog($"Shock weapon ({itemScript.itemProperties.itemName})", Plugin.spamType.message);

            foreach (PatcherTool zapgun in ZapGunMethods.zapGuns)
                if (zapgun.isShocking && zapgun.shockedTargetScript == this)
                {
                    if (charged)
                    {
                        zapgun.StopShockingAnomalyOnClient();
                        shockedByPlayer.DamagePlayer(15, causeOfDeath: CauseOfDeath.Electrocution);
                    }
                    else
                        chargeCoroutine = StartCoroutine(ChargeWeapon(zapgun));
                    break;
                }
        }

        public void StopShockingWithGun()
        {
            if (chargeCoroutine != null)
                StopCoroutine(chargeCoroutine);
        }

        private IEnumerator ChargeWeapon(PatcherTool zapgun) //TODO: change it
        {
            float chargeNeeded = zapgun.insertedBattery.charge - batteryChargeNeedUntilChargedState;

            while (zapgun.insertedBattery.charge > 0f && zapgun.insertedBattery.charge >= chargeNeeded)
            {
                yield return new WaitForEndOfFrame();
            }

            var shovelNORef = new NetworkObjectReference(GetNetworkObject());
            GameNetworkManagerPatch.hostNetHandler.SyncShovelDamageServerRpc(shovelNORef);
            //TODO: particle
            zapgun.StopShockingAnomalyOnClient(true);
        }

        private IEnumerator WaitUntilChargeRunsOut(float seconds)
        {
            yield return new WaitForSeconds(seconds);
            charged = false;
            itemScript.shovelHitForce /= 2;

        }

        internal void SyncDamageOnLocalClient(float seconds = 0)
        {
            charged = true;
            itemScript.shovelHitForce *= 2;
            StartCoroutine(WaitUntilChargeRunsOut(seconds));
        }
    }
}
