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
            if (UZGConfig.enemyListArray.Contains(___mainScript.enemyType.enemyName))
            {
                StartOfRound.Instance.StartCoroutine(ZapGunMethods.WaitAndDoSmth(___mainScript, __instance));
            }
        }
    }
}
