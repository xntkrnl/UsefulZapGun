using GameNetcodeStuff;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace UsefulZapGun.Methods
{
    internal class DOT
    {
        internal static IEnumerator DOTPlayer(PlayerControllerB player, int playerWhoHit, int damageToPlayer, float time, PatcherTool zapgun)
        {
            while (zapgun.shockedTargetScript == player && zapgun.isBeingUsed)
            {
                player.DamagePlayerFromOtherClientServerRpc(damageToPlayer, player.transform.position, playerWhoHit);
                yield return new WaitForSeconds(time);
            }
        }

        internal static IEnumerator DOTEnemy(EnemyAICollisionDetect enemyAICol, PlayerControllerB playerWhoHit, int force, float time, PatcherTool zapgun)
        {
            var enemyAIColIHittable = (IHittable)enemyAICol;

            while (zapgun.shockedTargetScript == enemyAICol && zapgun.isBeingUsed)
            {
                enemyAIColIHittable.Hit(force, enemyAICol.transform.position, playerWhoHit);
                yield return new WaitForSeconds(time);
            }
        }
    }
}
