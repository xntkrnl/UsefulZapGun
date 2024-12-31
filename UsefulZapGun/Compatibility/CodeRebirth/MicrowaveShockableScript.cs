using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Compatibility.CodeRebirth
{
    internal class MicrowaveShockableScript : MonoBehaviour, IShockableWithGun
    {
        bool IShockableWithGun.CanBeShocked()
        {
            throw new NotImplementedException();
        }

        float IShockableWithGun.GetDifficultyMultiplier()
        {
            throw new NotImplementedException();
        }

        NetworkObject IShockableWithGun.GetNetworkObject()
        {
            throw new NotImplementedException();
        }

        Vector3 IShockableWithGun.GetShockablePosition()
        {
            throw new NotImplementedException();
        }

        Transform IShockableWithGun.GetShockableTransform()
        {
            throw new NotImplementedException();
        }

        void IShockableWithGun.ShockWithGun(PlayerControllerB shockedByPlayer)
        {
            throw new NotImplementedException();
        }

        void IShockableWithGun.StopShockingWithGun()
        {
            throw new NotImplementedException();
        }

        IEnumerator ToggleMicrowaveAfterSomeTime()
        {
            yield return null;
            throw new NotImplementedException();
        }
    }
}
