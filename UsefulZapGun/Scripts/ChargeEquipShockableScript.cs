using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Scripts
{
    internal class ChargeEquipShockableScript : MonoBehaviour, IShockableWithGun
    {
        private GrabbableObject itemScript;
        private float chargeMultiplier;
        private Coroutine chargeCoroutine;
        private Coroutine damageCoroutine;

        private void Start()
        {
            itemScript = base.GetComponent<GrabbableObject>();
            chargeMultiplier = 1f; //for future
        }

        public bool CanBeShocked()
        {
            if (itemScript.insertedBattery.charge < 1f)
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
            {
                if (zapgun.isShocking && zapgun.shockedTargetScript == this)
                {
                    chargeCoroutine = StartCoroutine(ChargeItem(zapgun, itemScript));
                    break;
                }
            }
        }

        public void StopShockingWithGun()
        {
            if (chargeCoroutine != null)
                StopCoroutine(chargeCoroutine);
        }

        private IEnumerator ChargeItem(PatcherTool zapgun, GrabbableObject item)
        {
            if (item.insertedBattery.empty)
                item.insertedBattery.empty = false;

            while (item.insertedBattery.charge < 1f)
            {
                yield return new WaitForEndOfFrame();
                item.insertedBattery.charge += chargeMultiplier * (Time.deltaTime / zapgun.itemProperties.batteryUsage); //I think I need to optimize this but we'll see
                Plugin.SpamLog($"charge = {item.insertedBattery.charge}", Plugin.spamType.debug);
            }

            zapgun.StopShockingAnomalyOnClient();
        }
    }
}
