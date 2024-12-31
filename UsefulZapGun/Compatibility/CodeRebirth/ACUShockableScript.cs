using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;

namespace UsefulZapGun.Compatibility.CodeRebirth
{
    internal class ACUShockableScript : MonoBehaviour, IShockableWithGun
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

        IEnumerator DecreaseDetectionRangeEverySecond()
        {
            yield return null;
            throw new NotImplementedException();
            //decrese detection range every second (defaultDetectionRange/15 every second)
            //maybe explosion when <= 0 range
            //check if i can disable screen (prob not)
        }
    }
}
