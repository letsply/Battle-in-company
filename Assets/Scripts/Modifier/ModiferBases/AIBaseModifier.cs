using UnityEngine;
using static UnityEditor.Progress;

public class AIBaseModifier : BaseModifier
{
    protected EnemyBase enemy;
    public void FindEnemy(EnemyBase enemyToMod)
    {
        enemy = enemyToMod;
    }
}
