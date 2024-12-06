using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Unity.Netcode;
using UnityEngine;
using UsefulZapGun.Patches;

namespace UsefulZapGun.Methods
{
    internal class ZapGunMethods
    {
        internal static IEnumerator WaitAndDoSmth(EnemyAI enemyScript, EnemyAICollisionDetect instance)
        {
            yield return new WaitForSeconds(3f);

            if (enemyScript.stunNormalizedTimer > 0f)
            {

            }
        }
    }
}
