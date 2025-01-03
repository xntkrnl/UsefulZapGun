using CodeRebirth.src.Content.Maps;
using GameNetcodeStuff;
using System;
using System.Collections;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Compatibility.CodeRebirth.Scripts
{
    internal class LaserShockableScript : MonoBehaviour, IShockableWithGun
    {
        LaserTurret mainScript;
        bool isExploded;

        private void Start()
        {
            mainScript = GetComponent<LaserTurret>();
        }

        bool IShockableWithGun.CanBeShocked()
        {
            return !isExploded;
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
            PatcherTool zapgun = (PatcherTool)shockedByPlayer.currentlyHeldObjectServer;
            isExploded = true;
            StartCoroutine(Explode(zapgun));
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            return;
        }

        IEnumerator Explode(PatcherTool zapgun)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();
            zapgun.StopShockingAnomalyOnClient();
            yield return new WaitForEndOfFrame();
            Landmine.SpawnExplosion(mainScript.transform.position + new Vector3(0, 0.5f, 0), true, 2, 6, 20, 5);

            mainScript.impactAudioSource.Stop();
            mainScript.transform.Find("LaserIdleAudioSource").gameObject.SetActive(false); //i hate this
            mainScript.transform.Find("LaserTrapArmature").gameObject.SetActive(false);
            mainScript.transform.Find("Light (4)").gameObject.SetActive(false);
            mainScript.enabled = false;
        }
    }
}
