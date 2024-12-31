using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Text;
using UsefulZapGun.Methods;

namespace UsefulZapGun.Patches.Enemy
{
    internal class EnemyAIPatch
    {
        [HarmonyPostfix, HarmonyPatch(typeof(EnemyAI), "KillEnemyServerRpc")]
        static void KillEnemyServerRpcPatch(ref EnemyAI __instance)
        {
            foreach (PatcherTool zapgun in ZapGunMethods.zapGuns)
            {
                var targetScript = (EnemyAICollisionDetect)zapgun.shockedTargetScript;

                if (zapgun.isBeingUsed && targetScript.mainScript == __instance)
                {
                    zapgun.StopShockingAnomalyOnClient(true);
                    //this time i will admit that there may be more than 1 gun aimed at the enemy
                }
            }
        }
    }
}
