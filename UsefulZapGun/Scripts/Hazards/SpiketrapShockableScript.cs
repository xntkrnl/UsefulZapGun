using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Scripts.Hazards
{
    internal class SpiketrapShockableScript : MonoBehaviour, IShockableWithGun
    {
        internal int zapCount;
        bool canShock;
        SpikeRoofTrap spikeScript;
        Coroutine coroutine;

        private void Start()
        {
            zapCount = 0;
            canShock = true;
            spikeScript = base.transform.parent.Find("Trigger").GetComponent<SpikeRoofTrap>();
        }

        public bool CanBeShocked()
        {
            return canShock;
        }

        public float GetDifficultyMultiplier()
        {
            return 0.4f;
        }

        public NetworkObject GetNetworkObject()
        {
            return spikeScript.NetworkObject;
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
                    var NORef = new NetworkObjectReference(GetNetworkObject());
                    GameNetworkManagerPatch.hostNetHandler.SyncZapCountServerRpc(NORef, zapCount+1);
                    zapgun.StopShockingAnomalyOnClient(true);
                    break;
                }
        }

        public void StopShockingWithGun()
        {
            return;
        }

        internal void SyncCanShockOnLocalClient(bool sync)
        {
            canShock = sync;
            spikeScript.trapActive = sync;
            base.GetComponent<Light>().enabled = sync;
        }
    }
}
