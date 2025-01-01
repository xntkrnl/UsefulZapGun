using CodeRebirth.src.Content.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

namespace UsefulZapGun.Compatibility.CodeRebirth.Network
{
    internal class CodeRebirthNetwork : NetworkBehaviour
    {
        [ServerRpc(RequireOwnership = false)]
        internal void SyncACURangeServerRpc(NetworkBehaviourReference ACURef)
        {
            ACURef.TryGet(out AirControlUnit ACU);
            SyncACURangeClientRpc(ACURef, ACU.detectionRange);
        }

        [ClientRpc]
        private void SyncACURangeClientRpc(NetworkBehaviourReference ACURef, float range)
        {
            ACURef.TryGet(out AirControlUnit ACU);
            ACU.detectionRange = range;
        }

    }
}
