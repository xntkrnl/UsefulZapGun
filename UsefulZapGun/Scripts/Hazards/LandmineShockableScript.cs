﻿using GameNetcodeStuff;
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
        bool isShocked;

        private void Start()
        {
            mineScript = base.GetComponent<Landmine>();
        }

        public bool CanBeShocked()
        {
            return !mineScript.hasExploded && !isShocked;
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
            isShocked = shockedByPlayer != GameNetworkManager.Instance.localPlayerController;
        }

        public void StopShockingWithGun()
        {
            mineScript.ExplodeMineServerRpc();
        }
    }
}
