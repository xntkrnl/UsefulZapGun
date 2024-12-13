using GameNetcodeStuff;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Scripts
{
    internal class WeaponShockableScript : MonoBehaviour, IShockableWithGun
    {
        private Shovel itemScript;
        private bool charged;
        private Coroutine chargeCoroutine;
        private float batteryChargeNeedUntilChargedState;

        private void Start()
        {
            itemScript = base.GetComponent<Shovel>();
            charged = false;
            batteryChargeNeedUntilChargedState = UZGConfig.needForShovelCharge.Value / 100;

        }

        public bool CanBeShocked()
        {
            if (itemScript.playerHeldBy == null || batteryChargeNeedUntilChargedState > 1 || batteryChargeNeedUntilChargedState <= 0)
                return true;
            else return false;
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
            return base.transform.position;
        }

        public Transform GetShockableTransform()
        {
            return base.transform;
        }

        public void ShockWithGun(PlayerControllerB shockedByPlayer)
        {
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

        private IEnumerator ChargeWeapon(PatcherTool zapgun)
        {
            float chargeNeeded = zapgun.insertedBattery.charge - batteryChargeNeedUntilChargedState;

            while (zapgun.insertedBattery.charge > 0f && zapgun.insertedBattery.charge >= chargeNeeded)
            {   
                yield return new WaitForEndOfFrame();
            }

            charged = true;
            itemScript.shovelHitForce *= 2;
            StartCoroutine(WaitUntilChargeRunsOut());
            //TODO: particle
            zapgun.StopShockingAnomalyOnClient(true);
        }

        private IEnumerator WaitUntilChargeRunsOut()
        {
            yield return new WaitForSeconds(UZGConfig.chargeLifeTime.Value);
            charged = false;
            itemScript.shovelHitForce /= 2;
            
        }
    }
}
