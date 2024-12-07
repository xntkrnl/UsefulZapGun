using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Scripts
{
    internal class TestEquipShockableScript : MonoBehaviour, IShockableWithGun
    {
        GrabbableObject itemScript;
        Battery battery;


        private void Start()
        {
            itemScript = base.GetComponent<GrabbableObject>();
            battery = itemScript.insertedBattery;
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
            battery.charge += 1f;
            Plugin.SpamLog($"charge = {battery.charge}", Plugin.spamType.info);
        }

        public void StopShockingWithGun()
        {
            return;
        }
    }
}
