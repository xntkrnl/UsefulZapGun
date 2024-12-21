using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Patches
{
    internal class EnemyAICollisionDetectPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(EnemyAICollisionDetect), "IShockableWithGun.ShockWithGun")]
        static void ShockWithGunPatch(ref EnemyAI ___mainScript, ref EnemyAICollisionDetect __instance)
        {
            if (UZGConfig.timeDict.ContainsKey(___mainScript.enemyType))
                StartOfRound.Instance.StartCoroutine(ZapGunMethods.WaitAndExplode(___mainScript, UZGConfig.timeDict[___mainScript.enemyType].Value));
        }
    }
}
