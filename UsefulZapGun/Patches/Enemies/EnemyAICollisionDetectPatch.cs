using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
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

            /*if (___mainScript is BlobAI)
            {
                ZapGunMethods.StartEvaporation(__instance, (BlobAI)___mainScript);
            }*/

            if (___mainScript is ForestGiantAI)
            {
                ZapGunMethods.StartFire(__instance, (ForestGiantAI)___mainScript);
            }
        }
    }
}
