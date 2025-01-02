using GameNetcodeStuff;
using System.Collections;
using Unity.Netcode;
using UnityEngine;


namespace UsefulZapGun.Scripts.Hazards
{
    internal class TurretShockableScript : MonoBehaviour, IShockableWithGun
    {
        Turret turretScript;
        Coroutine coroutine;

        private void Start()
        {
            //canShock = true;
            turretScript = GetComponent<Turret>();
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
            return transform.position;
        }

        public Transform GetShockableTransform()
        {
            return transform;
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
