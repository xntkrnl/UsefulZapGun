using CodeRebirth.src.Content.Maps;
using GameNetcodeStuff;
using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.VFX;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Compatibility.CodeRebirth.Scripts
{
    internal class TeslaShockableScript : MonoBehaviour, IShockableWithGun
    {
        TeslaShock mainScript;
        Coroutine coroutine;
        internal float chargeNeeded;
        internal float charge;

        private void Start()
        {
            mainScript = GetComponent<TeslaShock>();
            chargeNeeded = 0.75f;
        }

        bool IShockableWithGun.CanBeShocked()
        {
            return mainScript.enabled;
        }

        float IShockableWithGun.GetDifficultyMultiplier()
        {
            return 0.8f;
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
            if (shockedByPlayer == GameNetworkManager.Instance.localPlayerController)
            {
                PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;

                coroutine = StartCoroutine(DrainChargeAndExplode(zapgun));
            }
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            if (coroutine != null)
            {
                StopCoroutine(coroutine);
                coroutine = null;
            }
        }

        IEnumerator DrainChargeAndExplode(PatcherTool zapgun)
        {
            while (zapgun.insertedBattery.charge > 0)
            {
                charge += Time.deltaTime / 22;

                if (charge >= chargeNeeded)
                {
                    NetworkBehaviourReference TeslaRef = new NetworkBehaviourReference(mainScript);
                    GameNetworkManagerPatch.rebirthNetwork.SyncTeslaServerRpc(charge, TeslaRef);

                    zapgun.StopShockingAnomalyOnClient();
                }

                yield return new WaitForEndOfFrame();
            }


            yield return new WaitForEndOfFrame();
            Landmine.SpawnExplosion(zapgun.transform.position, true, 2, 4, 30);
        }

        internal void DisableMainScriptOnLocalClient()
        {
            mainScript.teslaIdleAudioSource.Stop();
            mainScript.teslaAudioSource.Stop();
            mainScript.vfx.Stop();
            mainScript.enabled = false;
            mainScript.transform.Find("vg_PlasmaSphere").gameObject.SetActive(false);
        }
    }
}
