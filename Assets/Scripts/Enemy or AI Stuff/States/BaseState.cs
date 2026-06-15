using System.Collections;
using UnityEngine;

public abstract class BaseState
{
    protected EnemyBase2 enemyBase2;

    public void StartState(EnemyBase2 stateMachineControler)
    {
        enemyBase2 = stateMachineControler;

        StartOverride();
    }
    protected virtual void StartOverride()
    {

    }

    public void OnUpdate()
    {
        UpdateOverride();
    }
    protected virtual void UpdateOverride()
    {

    }

    protected virtual void EndState()
    {
        enemyBase2.SwitchState(EnemyBase2.States.Idle);
    }

    public IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);
    }

}

