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
            return (itemScript.insertedBattery.charge < 1f && itemScript.playerHeldBy == null) || chargeMultiplier <= 0;
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
            return itemScript.transform.position + new Vector3(0, 0.2f, 0);
        }

        public Transform GetShockableTransform()
        {
            return transform;
        }

        public void ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog($"Shock item with battery ({itemScript.itemProperties.itemName})", Plugin.spamType.message);

            itemScript.grabbable = false;
            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            chargeCoroutine = StartCoroutine(ChargeItem(zapgun, itemScript));

        }

        public void StopShockingWithGun()
        {
            StopCoroutine(chargeCoroutine);
            itemScript.grabbable = true;
            Plugin.SpamLog($"charge = {itemScript.insertedBattery.charge}", Plugin.spamType.debug);
            itemScript.SyncBatteryServerRpc((int)(itemScript.insertedBattery.charge * 100));
        }

        private IEnumerator ChargeItem(PatcherTool zapgun, GrabbableObject item) //TODO: slightly change it
        {
            if (item.insertedBattery.empty)
                item.insertedBattery.empty = false;

            while (item.insertedBattery.charge < 1f)
            {
                yield return new WaitForEndOfFrame();
                item.insertedBattery.charge += chargeMultiplier * (Time.deltaTime / item.itemProperties.batteryUsage);
            }

            zapgun.StopShockingAnomalyOnClient(true);
        }
    }
}
