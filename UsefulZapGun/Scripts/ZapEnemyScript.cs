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
            base.gameObject.GetComponent<EnemyAI>().enemyType.canBeStunned = true;
        }
    }
}
