using CodeRebirth.src.Content.Maps;
using GameNetcodeStuff;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Compatibility.CodeRebirth.Scripts
{
    internal class IndustrialFanShockableScript : MonoBehaviour, IShockableWithGun
    {
        IndustrialFan mainScript;
        Animator animator;
        float defaultSuckForce;
        float defaultPushForce;
        float defaultRotationSpeed;

        bool isShockedByAnotherClient;

        private void Start()
        {
            animator = GetComponent<Animator>();
            mainScript = GetComponent<IndustrialFan>();
            defaultSuckForce = mainScript.suctionForce;
            defaultPushForce = mainScript.pushForce;
            defaultRotationSpeed = mainScript.rotationSpeed;
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

            mainScript.windAudioSource.Pause();
            mainScript.rotationSpeed = 0;
            mainScript.pushForce = 0;
            mainScript.suctionForce = 0;
            animator.enabled = false;
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            isShockedByAnotherClient = false;

            mainScript.windAudioSource.UnPause();
            mainScript.rotationSpeed = defaultRotationSpeed;
            mainScript.pushForce = defaultPushForce;
            mainScript.suctionForce = defaultSuckForce;
            animator.enabled = true;
        }
    }
}
