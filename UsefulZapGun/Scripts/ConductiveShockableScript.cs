using GameNetcodeStuff;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Scripts
{
    internal class ConductiveShockableScript : MonoBehaviour, IShockableWithGun
    {
        GrabbableObject itemScript;

        private void Start()
        {
            itemScript = base.GetComponent<GrabbableObject>();
        }

        public bool CanBeShocked()
        {
            if (itemScript.playerHeldBy == null)
                return true;
            return false;
        }

        public float GetDifficultyMultiplier()
        {
            return 0f;
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
                    zapgun.StopShockingAnomalyOnClient(true);
                    shockedByPlayer.DamagePlayer(15, causeOfDeath: CauseOfDeath.Electrocution);
                    break;
                }
        }

        public void StopShockingWithGun()
        {
            return;
        }
    }
}
