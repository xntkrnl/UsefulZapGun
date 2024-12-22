using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;

namespace UsefulZapGun.Patches
{
    internal class PlayerControllerBPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(PlayerControllerB), "IShockableWithGun.ShockWithGun")]
        static void ShockWithGunPatch(ref PlayerControllerB __instance, PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog("Shock Player", Plugin.spamType.message);

            if (UZGConfig.enableDOTPlayers.Value)
            {
                var playerNBRef = new NetworkBehaviourReference(__instance);
                GameNetworkManagerPatch.hostNetHandler.DOTPlayerServerRpc(__instance, (int)shockedByPlayer.playerClientId);
            }
        }
    }
}
