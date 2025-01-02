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
        SpikeRoofTrap spikeScript;
        Coroutine coroutine;
        bool localPlayer;

        private void Start()
        {
            zapCount = 0;
            spikeScript = transform.parent.Find("Trigger").GetComponent<SpikeRoofTrap>();
        }

        public bool CanBeShocked()
        {
            return spikeScript.trapActive && !localPlayer;
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
            return transform.position;
        }

        public Transform GetShockableTransform()
        {
            return transform;
        }

        public void ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog("Shock spiketrap", Plugin.spamType.message);

            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            coroutine = StartCoroutine(WaitAndStopShocking(zapgun));
            localPlayer = shockedByPlayer == GameNetworkManager.Instance.localPlayerController;
        }

        public void StopShockingWithGun()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
            localPlayer = false;
        }

        internal void SyncCanShockOnLocalClient(bool sync)
        {
            spikeScript.trapActive = sync;
            GetComponent<Light>().enabled = sync;
        }

        private IEnumerator WaitAndStopShocking(PatcherTool zapgun)
        {
            yield return new WaitForSeconds(1f);
            if (zapgun.shockedTargetScript == this && zapgun.isBeingUsed && localPlayer)
            {
                var NORef = new NetworkObjectReference(GetNetworkObject());
                GameNetworkManagerPatch.hostNetHandler.SyncZapCountServerRpc(NORef, zapCount + 1);
                zapgun.StopShockingAnomalyOnClient(true);
            }
        }
    }
}
