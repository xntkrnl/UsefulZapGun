using CodeRebirth.src.Content.Maps;
using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Compatibility.CodeRebirth.Patches;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Compatibility.CodeRebirth.Scripts
{
    internal class FlashShockableScript : MonoBehaviour, IShockableWithGun
    {
        private FlashTurret mainScript;
        private float initialCooldown;
        private float maxCooldown;
        private Coroutine coroutine;
        private bool isStunnedByLocalClient;

        private void Start()
        {
            mainScript = GetComponent<FlashTurret>();
            initialCooldown = mainScript.flashCooldown;
            maxCooldown = 60;

        }

        bool IShockableWithGun.CanBeShocked()
        {
            return mainScript.flashCooldown < maxCooldown && coroutine == null;
        }

        float IShockableWithGun.GetDifficultyMultiplier()
        {
            return 1.5f;
        }

        NetworkObject IShockableWithGun.GetNetworkObject()
        {
            return mainScript.NetworkObject;
        }

        Vector3 IShockableWithGun.GetShockablePosition()
        {
            return mainScript.transform.position + new Vector3(0, 2f, 0);
        }

        Transform IShockableWithGun.GetShockableTransform()
        {
            return mainScript.transform;
        }

        void IShockableWithGun.ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            CRPlugin.SpamLog("Shock flash turret", CRPlugin.spamType.message);

            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            coroutine = StartCoroutine(IncreseCooldown(zapgun));
            mainScript.detectionRange *= 5;
            mainScript.rotationSpeed /= 5;
            isStunnedByLocalClient = shockedByPlayer == GameNetworkManager.Instance.localPlayerController;
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            CRPlugin.SpamLog("Stop zaping flash turret!", CRPlugin.spamType.debug);
            StopCoroutine(coroutine);
            coroutine = null;
            mainScript.detectionRange /= 5;
            mainScript.rotationSpeed *= 5;
            if (isStunnedByLocalClient)
            {
                NetworkBehaviourReference FlashRef = new NetworkBehaviourReference(mainScript);
                CodeRebirthGameNetworkManagerPatch.rebirthNetwork.SyncFlashCooldownServerRpc(FlashRef);
                isStunnedByLocalClient = false;
            }
        }

        IEnumerator IncreseCooldown(PatcherTool zapgun)
        {
            while (mainScript.flashCooldown < maxCooldown)
            {
                mainScript.flashCooldown += Time.deltaTime;
                yield return new WaitForEndOfFrame();
            }

            if (maxCooldown > mainScript.flashCooldown)
                mainScript.flashCooldown = maxCooldown;

            yield return new WaitForEndOfFrame();
            zapgun.StopShockingAnomalyOnClient(true);
        }
    }
}
