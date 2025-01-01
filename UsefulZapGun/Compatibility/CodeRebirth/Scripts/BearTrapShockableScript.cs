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
            var position = mainScript.transform.position;
            position = new Vector3(position.x, position.y + 0.5f, position.z);

            return position;
        }

        Transform IShockableWithGun.GetShockableTransform()
        {
            return mainScript.transform;
        }

        void IShockableWithGun.ShockWithGun(PlayerControllerB shockedByPlayer)
        {
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
        }
    }
}
