using System;
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

    protected virtual void EndState(EnemyBase2.States state)
    {
        enemyBase2.SwitchState(state);
    }

    public virtual IEnumerator Wait(float time, Coroutine coroutine)
    {
        yield return new WaitForSeconds(time);
        coroutine = null;
    }

}

