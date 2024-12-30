using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Scripts.Hazards
{
    internal class LandmineShockableScript : MonoBehaviour, IShockableWithGun
    {
        Landmine mineScript;
        bool canShock;

        private void Start()
        {
            canShock = true;
            mineScript = base.GetComponent<Landmine>();
        }

        public bool CanBeShocked()
        {
            return canShock;
        }

        public float GetDifficultyMultiplier()
        {
            return 0f;
        }

        public NetworkObject GetNetworkObject()
        {
            return mineScript.NetworkObject;
        }

        public Vector3 GetShockablePosition()
        {
            var position = base.transform.position;
            position = new Vector3(position.x, position.y + 0.5f, position.z);

            return position;
        }

        public Transform GetShockableTransform()
        {
            return base.transform;
        }

        public void ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog("Shock landmine", Plugin.spamType.message);

            foreach (PatcherTool zapgun in ZapGunMethods.zapGuns)
                if (zapgun.isBeingUsed && zapgun.shockedTargetScript == this)
                {
                    StartCoroutine(WaitAndExplode(zapgun));
                    break;
                }
        }

        public void StopShockingWithGun()
        {
            return;
        }

        private IEnumerator WaitAndExplode(PatcherTool zapgun)
        {
            zapgun.StopShockingAnomalyOnClient(true);
            canShock = false;
            yield return new WaitForSeconds(0.1f);
            mineScript.ExplodeMineServerRpc();
        }
    }
}
