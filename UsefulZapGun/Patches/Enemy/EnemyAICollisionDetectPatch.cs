using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Patches.Enemy
{
    internal class EnemyAICollisionDetectPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(EnemyAICollisionDetect), "IShockableWithGun.ShockWithGun")]
        static void ShockWithGunPatch(ref EnemyAICollisionDetect __instance, PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog("Shock Enemy", Plugin.spamType.message);

            if (UZGConfig.timeDict.ContainsKey(__instance.mainScript.enemyType) && UZGConfig.enableExplosion.Value)
            {
                //StartOfRound.Instance.StartCoroutine(ZapGunMethods.WaitAndExplode(___mainScript, UZGConfig.timeDict[___mainScript.enemyType].Value));
                ZapGunMethods.WaitAndExplode(__instance, __instance.mainScript, UZGConfig.timeDict[__instance.mainScript.enemyType].Value);
                return;
            }
            else
            {
                var enemyNBRef = new NetworkBehaviourReference(__instance.mainScript);
                var playerNBRef = new NetworkBehaviourReference(shockedByPlayer);
                GameNetworkManagerPatch.hostNetHandler.DOTEnemyServerRpc(enemyNBRef, playerNBRef);
            }

            if (__instance.mainScript is ForestGiantAI && UZGConfig.setForestGiantsOnFire.Value && !UZGConfig.timeDict.ContainsKey(__instance.mainScript.enemyType))
            {
                ZapGunMethods.StartFire(__instance, (ForestGiantAI)__instance.mainScript);
                return;
            }

            if (__instance.mainScript is BlobAI && UZGConfig.evaporateBlob.Value && !UZGConfig.timeDict.ContainsKey(__instance.mainScript.enemyType))
            {
                ZapGunMethods.StartEvaporation(__instance, (BlobAI)__instance.mainScript);
                return;
            }
        }

        [HarmonyPostfix, HarmonyPatch(typeof(EnemyAICollisionDetect), "IShockableWithGun.GetDifficultyMultiplier")]
        static void GetDifficultyMultiplierPatch(ref EnemyAICollisionDetect __instance, ref float __result)
        {
            if (!UZGConfig.enableDifficultyMultiplierPatch.Value || UZGConfig.distanceDivider.Value == 0)
                return;

            Plugin.SpamLog("GetDifficultyMultilier before = " + __result, Plugin.spamType.debug);
            __result *= Vector3.Distance(GameNetworkManager.Instance.localPlayerController.transform.position, __instance.mainScript.transform.position) / UZGConfig.distanceDivider.Value;
            Plugin.SpamLog("GetDifficultyMultilier after = " + __result, Plugin.spamType.debug);
        }
    }
}
