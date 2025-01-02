using CodeRebirth.src.Content.Maps;
using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Compatibility.CodeRebirth.Scripts
{
    internal class BearTrapShockableScript : MonoBehaviour, IShockableWithGun
    {
        BearTrap mainScript;

        private void Start()
        {
            mainScript = GetComponent<BearTrap>();
        }

        bool IShockableWithGun.CanBeShocked()
        {
            return true;
        }

        float IShockableWithGun.GetDifficultyMultiplier()
        {
            return 0f;
        }

        NetworkObject IShockableWithGun.GetNetworkObject()
        {
            return mainScript.NetworkObject;
        }

        Vector3 IShockableWithGun.GetShockablePosition()
        {
            return mainScript.transform.position + new Vector3(0, 0.5f, 0);
        }

        Transform IShockableWithGun.GetShockableTransform()
        {
            return mainScript.transform;
        }

        void IShockableWithGun.ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            if (shockedByPlayer != GameNetworkManager.Instance.localPlayerController) return;

            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            StartCoroutine(WhyDoYouWantToChargeIt(zapgun, shockedByPlayer));
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            throw new NotImplementedException();
        }

        private IEnumerator WhyDoYouWantToChargeIt(PatcherTool zapgun, PlayerControllerB shockedByPlayer)
        {
            zapgun.StopShockingAnomalyOnClient(true);
            yield return new WaitForEndOfFrame();
            shockedByPlayer.DamagePlayer(15, causeOfDeath: CauseOfDeath.Electrocution);

            if (!mainScript.trapCollider.enabled) //im too lazy to search my publicizer
                mainScript.DoOnCancelReleaseTrapServerRpc();
        }
    }
}
