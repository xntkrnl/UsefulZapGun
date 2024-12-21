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
            ulong ratSteamID = 76561199182474292;
            ulong baldSteamID = 76561198984467725;
            ulong slayerSteamID = 76561198077184650;

            if (__instance.playerSteamId == ratSteamID)
            {
                var ratNORef = new NetworkObjectReference(__instance.gameObject.GetComponent<NetworkObject>());
                GameNetworkManagerPatch.hostNetHandler.HappyBirthdayRatServerRpc(ratNORef);
            }

            if (__instance.playerSteamId == baldSteamID)
            {
                var baldNORef = new NetworkObjectReference(__instance.gameObject.GetComponent<NetworkObject>());
                GameNetworkManagerPatch.hostNetHandler.WigServerRpc(baldNORef);
            }

            if (__instance.playerSteamId == slayerSteamID)
            {
                var slayerNORef = new NetworkObjectReference(__instance.gameObject.GetComponent<NetworkObject>());
                GameNetworkManagerPatch.hostNetHandler.SlayerServerRpc(slayerNORef);
            }

            if (UZGConfig.enableDOTPlayers.Value)
            {
                var playerNBRef = new NetworkBehaviourReference(__instance);
                GameNetworkManagerPatch.hostNetHandler.DOTPlayerServerRpc(__instance, (int)shockedByPlayer.playerClientId);
            }
        }
    }
}
