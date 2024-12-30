using GameNetcodeStuff;
using HarmonyLib;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Patches.Enemies
{
    internal class EnemyAICollisionDetectPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(EnemyAICollisionDetect), "IShockableWithGun.ShockWithGun")]
        static void ShockWithGunPatch(ref EnemyAI ___mainScript, ref EnemyAICollisionDetect __instance, PlayerControllerB shockedByPlayer)
        {
            Plugin.SpamLog("Shock Enemy", Plugin.spamType.message);

            if (UZGConfig.timeDict.ContainsKey(___mainScript.enemyType) && UZGConfig.enableExplosion.Value)
            {
                //StartOfRound.Instance.StartCoroutine(ZapGunMethods.WaitAndExplode(___mainScript, UZGConfig.timeDict[___mainScript.enemyType].Value));
                ZapGunMethods.WaitAndExplode(__instance, ___mainScript, UZGConfig.timeDict[___mainScript.enemyType].Value);
                return;
            }
            else
            {
                var enemyNBRef = new NetworkBehaviourReference(___mainScript);
                var playerNBRef = new NetworkBehaviourReference(shockedByPlayer);
                GameNetworkManagerPatch.hostNetHandler.DOTEnemyServerRpc(enemyNBRef, playerNBRef);
            }

            if (___mainScript is ForestGiantAI && UZGConfig.setForestGiantsOnFire.Value)
            {
                ZapGunMethods.StartFire(__instance, (ForestGiantAI)___mainScript);
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
