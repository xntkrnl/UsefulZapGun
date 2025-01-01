using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Scripts.Items
{
    internal class ConductiveShockableScript : MonoBehaviour, IShockableWithGun
    {
        GrabbableObject itemScript;

        private void Start()
        {
            itemScript = GetComponent<GrabbableObject>();
        }

        public bool CanBeShocked()
        {
            return itemScript.playerHeldBy == null;
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
            return transform.position;
        }

        public Transform GetShockableTransform()
        {
            return transform;
        }

        public void ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog($"Shock conductive item ({itemScript.itemProperties.itemName})", Plugin.spamType.message);

            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            StartCoroutine(WaitFrameAndDamage(zapgun, shockedByPlayer));
        }

        private IEnumerator WaitFrameAndDamage(PatcherTool zapgun, PlayerControllerB shockedByPlayer)
        {
            zapgun.StopShockingAnomalyOnClient(true);
            yield return new WaitForEndOfFrame();
            shockedByPlayer.DamagePlayer(15, causeOfDeath: CauseOfDeath.Electrocution);
        }

        public void StopShockingWithGun()
        {
            return;
        }
    }
}
