using GameNetcodeStuff;
using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.Netcode;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Patches
{
    internal class EnemyAICollisionDetectPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(EnemyAICollisionDetect), "IShockableWithGun.ShockWithGun")]
        static void ShockWithGunPatch(ref EnemyAI ___mainScript, ref EnemyAICollisionDetect __instance, PlayerControllerB shockedByPlayer)
        {
            if (UZGConfig.timeDict.ContainsKey(___mainScript.enemyType) && UZGConfig.enableExplosion.Value)
            {
                //StartOfRound.Instance.StartCoroutine(ZapGunMethods.WaitAndExplode(___mainScript, UZGConfig.timeDict[___mainScript.enemyType].Value));
                ZapGunMethods.WaitAndExplode(__instance, ___mainScript, UZGConfig.timeDict[___mainScript.enemyType].Value);
            }
            else
            {
                var enemyNBRef = new NetworkBehaviourReference(___mainScript);
                var playerNBRef = new NetworkBehaviourReference(shockedByPlayer);
                GameNetworkManagerPatch.hostNetHandler.DOTEnemyServerRpc(enemyNBRef, playerNBRef);
            }
        }
    }
}
