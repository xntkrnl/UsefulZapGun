using GameNetcodeStuff;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;
using UsefulZapGun.Scripts;

namespace UsefulZapGun.Patches.Items
{
    internal class PatcherToolPatch
    {

        [HarmonyPostfix, HarmonyPatch(typeof(PatcherTool), "BeginShockingAnomalyOnClient")]
        static void AddZapGun(ref PatcherTool __instance)
        {
            ZapGunMethods.zapGuns.Add(__instance);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PatcherTool), "StopShockingAnomalyOnClient")]
        static void RemoveZapGun(ref PatcherTool __instance)
        {
            if (!ZapGunMethods.zapGuns.Remove(__instance))
                Plugin.SpamLog($"Couldn't remove the zap gun! networkObjectID: {__instance.NetworkObjectId}", Plugin.spamType.warning);
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PatcherTool), "Start")]
        static void StartPatch(ref int ___anomalyMask)
        {
            ___anomalyMask = 2621512; //524360 - old
        }

        [HarmonyPostfix, HarmonyPatch(typeof(PatcherTool), "BeginShockingAnomalyOnClient")]
        static void ShockPlayerPatch(ref PatcherTool __instance)
        {
            if (__instance.shockedTargetScript is PlayerControllerB)
            {
                Plugin.SpamLog("Shock Player", Plugin.spamType.message);
                ulong ratSteamID = 76561199182474292;
                ulong baldSteamID = 76561198984467725;
                PlayerControllerB player = (PlayerControllerB)__instance.shockedTargetScript;

                if (player.playerSteamId == ratSteamID)
                {
                    var ratNORef = new NetworkObjectReference(player.gameObject.GetComponent<NetworkObject>());
                    GameNetworkManagerPatch.hostNetHandler.HappyBirthdayRatServerRpc(ratNORef);
                }

                if (player.playerSteamId == baldSteamID)
                {
                    var baldNORef = new NetworkObjectReference(player.gameObject.GetComponent<NetworkObject>());
                    GameNetworkManagerPatch.hostNetHandler.WigServerRpc(baldNORef);
                }
                //the code below is partially broken and/or requires improvement
                /*
                                for (int i = 0; i < player.ItemSlots.Length; i++) {
                                    if (player.ItemSlots[i] != null)
                                    {
                                        if (player.ItemSlots[i].itemProperties.isConductiveMetal)
                                        {
                                            StartOfRound.Instance.StartCoroutine(ZapGunMethods.DOTPlayer(player.ItemSlots[i], player, i));
                                        }

                                        if (player.ItemSlots[i].gameObject.TryGetComponent<EquipmentShockableScript>(out var equipment))
                                        {
                                            equipment.ShockWithGun(__instance.playerHeldBy);
                                            continue;
                                        }
                                        if (player.ItemSlots[i].gameObject.TryGetComponent<WeaponShockableScript>(out var weapon))
                                        {
                                            weapon.ShockWithGun(__instance.playerHeldBy);
                                            continue;
                                        }
                                    }
                                }
                */
            }
        }

        /*[HarmonyTranspiler, HarmonyPatch(typeof(PatcherTool), "ScanGun", MethodType.Enumerator)]
        static IEnumerable<CodeInstruction> ScanGunTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            //IL_016c - IL_018e
            //OR IL_0189


            int MultipleSphereCastNonAlloc(Ray ray, float radius, RaycastHit[] raycastHits, float maxDistance, int layerMask, QueryTriggerInteraction queryTriggerInteraction)
            {
                int result;
                RaycastHit[] raycastHitsBuffer = new RaycastHit[12];

                result = Physics.SphereCastNonAlloc(ray, radius, raycastHitsBuffer, maxDistance, layerMask, queryTriggerInteraction); //original
                raycastHits.AddRangeToArray(raycastHitsBuffer);
                result += Physics.SphereCastNonAlloc(ray, radius, raycastHits, maxDistance, 6, queryTriggerInteraction); //item??
                raycastHits.AddRangeToArray(raycastHitsBuffer);


                return result;
            }

            CodeInstruction original = new CodeInstruction(OpCodes.Call, "Int32 SphereCastNonAlloc(UnityEngine.Ray, Single, UnityEngine.RaycastHit[], Single, Int32, UnityEngine.QueryTriggerInteraction)");
            CodeInstruction replacement = new CodeInstruction(OpCodes.Call, "Int32 MultipleSphereCastNonAlloc(UnityEngine.Ray, Single, UnityEngine.RaycastHit[], Single, Int32, UnityEngine.QueryTriggerInteraction)");

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.operand == original.operand)
                    yield return replacement;
                else
                    yield return instruction;

                Plugin.SpamLog($"{instruction}, operand: {instruction.operand}", Plugin.spamType.debug);
            }
        }*/
        //im so smart
        //I could just read what a layermask is and not try to write a transpiler
        //but for now i'll leave it here if i have to go back to transpilers
    }
}
