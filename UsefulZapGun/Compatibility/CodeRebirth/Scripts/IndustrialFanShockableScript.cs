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
    internal class IndustrialFanShockableScript : MonoBehaviour, IShockableWithGun
    {
        IndustrialFan mainScript;
        bool isShockedByAnotherClient;

        private void Start()
        {
            mainScript = GetComponent<IndustrialFan>();
        }

        bool IShockableWithGun.CanBeShocked()
        {
            return !isShockedByAnotherClient;
        }

        float IShockableWithGun.GetDifficultyMultiplier()
        {
            return 0.9f;
        }

        NetworkObject IShockableWithGun.GetNetworkObject()
        {
            return mainScript.NetworkObject;
        }

        Vector3 IShockableWithGun.GetShockablePosition()
        {
            return transform.position + new Vector3(0, 1f, 0);
        }

        Transform IShockableWithGun.GetShockableTransform()
        {
            return mainScript.transform;
        }

        void IShockableWithGun.ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            if (shockedByPlayer != GameNetworkManager.Instance.localPlayerController)
                isShockedByAnotherClient = true;
            mainScript.enabled = false;
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            mainScript.enabled = true;
            isShockedByAnotherClient = false;
        }
    }
}
