using UnityEngine;
using static UnityEditor.Progress;

public class AIBaseModifier : BaseModifier
{
    protected StateMachineControler enemy;
    public void FindEnemy(StateMachineControler enemyToMod)
    {
        enemy = enemyToMod;
    }
}
