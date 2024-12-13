using GameNetcodeStuff;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Scripts
{
    internal class EquipmentShockableScript : MonoBehaviour, IShockableWithGun
    {
        private GrabbableObject itemScript;
        private float chargeMultiplier;
        private Coroutine chargeCoroutine;

        private void Start()
        {
            itemScript = base.GetComponent<GrabbableObject>();
            var name = itemScript.itemProperties.itemName;
            chargeMultiplier = UZGConfig.CreateAndCheckConfigEntry(name, itemScript.itemProperties.batteryUsage/22);
        }

        public bool CanBeShocked()
        {
            if (itemScript.insertedBattery.charge < 1f || itemScript.playerHeldBy == null)
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
                item.insertedBattery.charge += chargeMultiplier * (Time.deltaTime / item.itemProperties.batteryUsage); //I think I need to change this but we'll see
                Plugin.SpamLog($"charge = {item.insertedBattery.charge}", Plugin.spamType.debug);
            }

            zapgun.StopShockingAnomalyOnClient(true);
        }
    }
}
