using UnityEngine;
using static UnityEditor.Progress;

public class AIBaseModifier : BaseModifier
{
    protected EnemyBase2 enemy;
    public void FindEnemy(EnemyBase2 enemyToMod)
    {
        enemy = enemyToMod;
    }
}
