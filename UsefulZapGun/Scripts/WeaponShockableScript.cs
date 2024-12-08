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
        private Coroutine damageCoroutine; //for future
        private void Start()
        {
            itemScript = base.GetComponent<Shovel>();
            charged = false;
        }

        public bool CanBeShocked()
        {
            return true;
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
            yield return new WaitForSeconds(3f); //make config
            if (zapgun.isShocking && zapgun.shockedTargetScript == this)
            {
                charged = true;
                itemScript.shovelHitForce *= 2; //make config
                StartCoroutine(WaitUntilChargeRunsOut());
                //TODO: particle
                zapgun.StopShockingAnomalyOnClient();
            }

        }

        private IEnumerator WaitUntilChargeRunsOut()
        {
            yield return new WaitForSeconds(15f); //make config
            charged = false;
            itemScript.shovelHitForce /= 2; //make config
            
        }
    }
}
