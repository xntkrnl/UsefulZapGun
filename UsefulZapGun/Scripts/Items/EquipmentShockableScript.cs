using GameNetcodeStuff;
using System.Collections;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Scripts.Items
{
    internal class EquipmentShockableScript : MonoBehaviour, IShockableWithGun
    {
        private GrabbableObject itemScript;
        private float chargeMultiplier;
        private Coroutine chargeCoroutine;

        private void Start()
        {
            itemScript = GetComponent<GrabbableObject>();
            var name = itemScript.itemProperties.itemName;
            chargeMultiplier = UZGConfig.multiplayerDict[itemScript.itemProperties].Value;
        }

        public bool CanBeShocked()
        {
            if (itemScript.insertedBattery.charge < 1f || itemScript.playerHeldBy == null || chargeMultiplier <= 0)
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
                //Plugin.SpamLog($"charge = {item.insertedBattery.charge}", Plugin.spamType.debug);
            }

            zapgun.StopShockingAnomalyOnClient(true);
        }
    }
}
