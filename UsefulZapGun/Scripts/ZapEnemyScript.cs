using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UsefulZapGun.Scripts
{
    internal class ZapEnemyScript : MonoBehaviour
    {
        private void Start()
        {
            var enemyGO = base.gameObject;
            var enemyComponent = enemyGO.GetComponent<EnemyAI>();

            if (enemyComponent.enemyType.canBeStunned)
                return;

            enemyComponent.enemyType.canBeStunned = true;
            Plugin.SpamLog($"{enemyGO.name} can now be stunned!", Plugin.spamType.info);
        }
    }
}
