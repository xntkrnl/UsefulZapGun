using HarmonyLib;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UsefulZapGun.Patches
{
    internal class UZGPatches
    {
        static bool alreadyPatched = false;

        [HarmonyPostfix, HarmonyPatch(typeof(MenuManager), "Start")]
        static void MenuManagerStartPatch()
        {
            if (alreadyPatched)
                return;

            var enemyArray = Resources.FindObjectsOfTypeAll<EnemyType>(); //thanks, Zaggy1024
            foreach (EnemyType enemy in enemyArray)
            {
                Plugin.SpamLog($"{enemy.enemyName} has been found!", Plugin.spamType.message);

                if (enemy.enemyName != "Red Locust Bees")
                    continue;
                else
                {
                    enemy.canBeStunned = true;
                    Plugin.SpamLog($"{enemy.enemyName} can be stunned now!", Plugin.spamType.info);
                }
            }

            alreadyPatched = true; //exiting to the menu launches this too, so we prevent the search just in case
        }

        [HarmonyPostfix, HarmonyPatch(typeof(EnemyAICollisionDetect), "IShockableWithGun.ShockWithGun")]
        static void ShockWithGunPatch(ref EnemyAI ___mainScript, ref EnemyAICollisionDetect __instance)
        {
            if (___mainScript.gameObject.name == "RedLocustBees(Clone)")
            {
                StartOfRound.Instance.StartCoroutine(WaitAndDoSmth(___mainScript, __instance));
            }
        }

        private static IEnumerator WaitAndDoSmth(EnemyAI beeScript, EnemyAICollisionDetect instance)
        {
            yield return new WaitForSeconds(3f);
            
            if (beeScript.stunNormalizedTimer > 0f)
            {
                var allZapGuns = GameObject.FindObjectsOfType<PatcherTool>(); //uuh i don't feel comfortable to do it at runtime
                foreach (PatcherTool zapgun in allZapGuns)
                {
                    if (zapgun.isBeingUsed && zapgun.shockedTargetScript == instance)
                    {
                        zapgun.StopShockingAnomalyOnClient(true);
                        break;
                    }
                }
                Landmine.SpawnExplosion(beeScript.gameObject.transform.position, true, 0, 3, 20, 3);
                StartOfRound.Destroy(beeScript.gameObject);
            }
        }
    }
}
