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
        internal void SyncACURangeServerRpc(NetworkBehaviourReference ACURef, bool disable = false)
        {
            ACURef.TryGet(out AirControlUnit ACU);
            if (disable)
                ACU.detectionRange = 0;

            SyncACURangeClientRpc(ACURef, ACU.detectionRange);
        }

        [ClientRpc]
        private void SyncACURangeClientRpc(NetworkBehaviourReference ACURef, float range)
        {
            ACURef.TryGet(out AirControlUnit ACU);
            Plugin.SpamLog($"Sync ACU range: {ACU.detectionRange} -> {range}", Plugin.spamType.info);
            ACU.detectionRange = range;
        }

        [ServerRpc(RequireOwnership = false)]
        internal void SyncFlashCooldownServerRpc(NetworkBehaviourReference FlashRef, bool disable = false)
        {
            FlashRef.TryGet(out FlashTurret Flash);
            if (disable)
                Flash.flashCooldown = 60;

            SyncFlashCooldownClientRpc(FlashRef, Flash.flashCooldown);
        }

        [ClientRpc]
        private void SyncFlashCooldownClientRpc(NetworkBehaviourReference FlashRef, float cooldown)
        {
            FlashRef.TryGet(out FlashTurret Flash);
            Plugin.SpamLog($"Sync FlashTurret CD: {Flash.flashCooldown} -> {cooldown}", Plugin.spamType.info);
            Flash.flashCooldown = cooldown;
        }
    }
}
