using CodeRebirth.src.Content.Maps;
using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Compatibility.CodeRebirth.Scripts
{
    internal class ACUShockableScript : MonoBehaviour, IShockableWithGun
    {
        private AirControlUnit mainScript;
        private float initialRange;
        private Coroutine coroutine;
        private bool localPlayer;

        private void Start()
        {
            mainScript = transform.parent.GetComponent<AirControlUnit>();
            initialRange = mainScript.detectionRange;
        }

        bool IShockableWithGun.CanBeShocked()
        {
            return mainScript.detectionRange > 0 && coroutine == null;
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
            var position = mainScript.transform.position;
            position = new Vector3(position.x, position.y + 2f, position.z);
            return position;
        }

        Transform IShockableWithGun.GetShockableTransform()
        {
            return mainScript.transform;
        }

        void IShockableWithGun.ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog("Shock ac unit", Plugin.spamType.message);

            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            coroutine = StartCoroutine(DecreaseDetectionRangeEverySecond(zapgun));
            mainScript.rotationSpeed /= 3;
            localPlayer = shockedByPlayer == GameNetworkManager.Instance.localPlayerController;
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            Plugin.SpamLog("Stop zaping ac unit!", Plugin.spamType.debug);
            StopCoroutine(coroutine);
            coroutine = null;
            mainScript.rotationSpeed *= 3;
            if (localPlayer)
            {
                NetworkBehaviourReference ACURef = new NetworkBehaviourReference(mainScript);
                GameNetworkManagerPatch.rebirthNetwork.SyncACURangeServerRpc(ACURef);
            }
        }

        IEnumerator DecreaseDetectionRangeEverySecond(PatcherTool zapgun)
        {
            while (mainScript.detectionRange > 0)
            {
                yield return new WaitForSeconds(1f);
                mainScript.detectionRange -= initialRange / 15;
            }

            yield return new WaitForEndOfFrame();
            zapgun.StopShockingAnomalyOnClient();
        }
    }
}
