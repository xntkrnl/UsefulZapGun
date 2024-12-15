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
        internal System.Random random;
        bool canShock;
        int totalWeight;
        int berserkWeight;
        int successWeight;
        Turret turretScript;
        Coroutine coroutine;

        private void Start()
        {
            berserkWeight = 
            totalWeight = berserkWeight + successWeight;
            canShock = true;
            turretScript = base.GetComponent<Turret>();
        }

        public bool CanBeShocked()
        {
            return canShock;
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
            foreach (PatcherTool zapgun in ZapGunMethods.zapGuns)
                if (zapgun.isShocking && zapgun.shockedTargetScript == this)
                    coroutine = StartCoroutine(Wait(zapgun, shockedByPlayer));
        }

        public void StopShockingWithGun()
        {
            if (coroutine != null)
                StopCoroutine(coroutine);
        }

        private IEnumerator Wait(PatcherTool zapgun, PlayerControllerB player)
        {
            yield return new WaitForSeconds(3f);
            StartCoroutine(BerserkAndDisable(player));
            zapgun.StopShockingAnomalyOnClient(true);
           
        }

        private IEnumerator BerserkAndDisable(PlayerControllerB player)
        {
            random = new System.Random((int)TimeOfDay.Instance.currentDayTime);
            Plugin.SpamLog($"time {TimeOfDay.Instance.currentDayTime} = {(int)TimeOfDay.Instance.currentDayTime}", Plugin.spamType.debug);
            var NORef = new NetworkObjectReference(GetNetworkObject());
            GameNetworkManagerPatch.hostNetHandler.SyncCanShockServerRpc(NORef, false);

            //TODO: smoke
            turretScript.turretMode = TurretMode.Berserk;
            turretScript.EnterBerserkModeServerRpc((int)player.playerClientId);
            yield return new WaitForSeconds(1.4f + 9f);

            turretScript.turretMode = TurretMode.Detection;
            turretScript.ToggleTurretServerRpc(false);
        }

        internal void SyncCanShockOnLocalClient(bool sync)
        {
            canShock = sync;
        }
    }
}
