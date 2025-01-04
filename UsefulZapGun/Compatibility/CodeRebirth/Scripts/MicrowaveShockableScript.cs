using CodeRebirth.src.Content.Maps;
using GameNetcodeStuff;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Compatibility.CodeRebirth.Scripts
{
    internal class MicrowaveShockableScript : MonoBehaviour, IShockableWithGun
    {
        FunctionalMicrowave mainScript;
        float microwaveOpeningTimer;
        float microwaveClosingTimer;
        bool isShockedByAnotherClient;

        private void Start()
        {
            mainScript = GetComponent<FunctionalMicrowave>();
            microwaveClosingTimer = mainScript.microwaveClosingTimer;
            microwaveOpeningTimer = mainScript.microwaveOpeningTimer;
        }

        bool IShockableWithGun.CanBeShocked()
        {
            return !isShockedByAnotherClient;
        }

        float IShockableWithGun.GetDifficultyMultiplier()
        {
            return 0;
        }

        NetworkObject IShockableWithGun.GetNetworkObject()
        {
            return mainScript.NetworkObject;
        }

        Vector3 IShockableWithGun.GetShockablePosition()
        {
            return mainScript.transform.position;
        }

        Transform IShockableWithGun.GetShockableTransform()
        {
            return mainScript.transform;
        }

        void IShockableWithGun.ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            //as i said somewhere, too lazy to publicize
            if (shockedByPlayer != GameNetworkManager.Instance.localPlayerController)
                isShockedByAnotherClient = true;

            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            StartCoroutine(ToggleMicrowave(zapgun));
                
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            return;
        }

        IEnumerator ToggleMicrowave(PatcherTool zapgun)
        {
            if (mainScript.animator.GetBool("isActivated"))
                mainScript.microwaveClosingTimer = 0;
            else
                mainScript.microwaveOpeningTimer = 0;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            mainScript.microwaveClosingTimer = microwaveClosingTimer;
            mainScript.microwaveOpeningTimer = microwaveOpeningTimer;
            zapgun.insertedBattery.charge = Mathf.Clamp(zapgun.insertedBattery.charge - 0.15f, 0f, 1f);
            zapgun.StopShockingAnomalyOnClient(true);
        }
    }
}
