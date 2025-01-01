using DigitalRuby.ThunderAndLightning;
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
    internal class TurretShockableScript : MonoBehaviour, IShockableWithGun
    {
        Turret turretScript;
        Coroutine coroutine;

        private void Start()
        {
            //canShock = true;
            turretScript = base.GetComponent<Turret>();
        }

        public bool CanBeShocked()
        {
            return turretScript.turretActive;
        }

        public float GetDifficultyMultiplier()
        {
            return 0.9f;
        }

        public NetworkObject GetNetworkObject()
        {
            return turretScript.NetworkObject;
        }

        public Vector3 GetShockablePosition()
        {
            return base.transform.position;
        }

        public Transform GetShockableTransform()
        {
            return base.transform;
        }

        public void ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog("Shock turret", Plugin.spamType.message);

            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            coroutine = StartCoroutine(Wait(zapgun, shockedByPlayer));
        }

        public void StopShockingWithGun()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        private IEnumerator Wait(PatcherTool zapgun, PlayerControllerB player)
        {
            yield return new WaitForSeconds(UZGConfig.timeNeedForTurretDisable.Value);
            if (zapgun.shockedTargetScript == this && zapgun.isBeingUsed)
            {
                StartCoroutine(BerserkAndDisable(player));
            }
        }

        private IEnumerator BerserkAndDisable(PlayerControllerB player)
        {
            //TODO: smoke
            turretScript.turretMode = TurretMode.Berserk;
            turretScript.EnterBerserkModeServerRpc((int)player.playerClientId);
            yield return new WaitForSeconds(1.4f + 9f);

            turretScript.turretMode = TurretMode.Detection;
            turretScript.ToggleTurretServerRpc(false);
        }
    }
}
