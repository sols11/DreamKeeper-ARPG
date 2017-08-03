using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SFramework;

namespace DreamKeeper
{
    public class KnightMono : IEnemyMono
    {
        EnemyKnight enemyKnight;

        public override void Initialize()
        {
            base.Initialize();
            enemyKnight=EnemyMedi.Enemy as EnemyKnight;
        }

    }
}