using System.Collections.Generic;
using UnityEngine;

public class Attack : BaseState
{
    private EnemyBase2.AttackType attackType;
    private EnemyBase2.StratagieType stratagie;
    private Coroutine coroutine;
    private float distance;

    protected override void StartOverride()
    {
        attackType = enemyBase2.GetAttackType();
        stratagie = enemyBase2.GetStratagie();
    }

    protected override void UpdateOverride()
    {
        distance = Vector3.Distance(enemyBase2.Agent.destination, enemyBase2.transform.position);
        if (attackType == EnemyBase2.AttackType.CloseRange)
        { CloseRangeCombat(); }
        else
        { LongRangeCombat(); }
    }

    void CloseRangeCombat()
    {
        if (stratagie == EnemyBase2.StratagieType.Strategic)
        {
            if (enemyBase2.GetValue<float>(EnemyValues.attackRange) > distance)
            {
                enemyBase2.Agent.isStopped = true;
                enemyBase2.Hit();
                EndState(EnemyBase2.States.Idle);
            }
            else
            {
                enemyBase2.Agent.destination = enemyBase2.GetValue<Transform>(EnemyValues.targetEnemy).position;
            }
        }
    }

    void LongRangeCombat()
    {
        if (stratagie == EnemyBase2.StratagieType.Strategic)
        {

            
         

            if ()
            {

                RaycastHit Throwhit;
                Ray ThrowRay = new Ray(enemyBase2.transform.position, enemyBase2.transform.TransformDirection(Vector3.forward));

                if (Physics.Raycast(ThrowRay, out Throwhit, enemyBase2.GetValue<float>(EnemyValues.attackRange)))
                {
                    if (Throwhit.collider.tag == "Player")
                    {
                        if (coroutine == null)
                        {
                            enemyBase2.Throw();
                        }
                        coroutine = enemyBase2.StartCoroutine(Wait(itemHolderAnim.GetCurrentAnimatorStateInfo(0).length));
                    }
                }

            }

        }
    }

    protected override void EndState(EnemyBase2.States state)
    {
        enemyBase2.Agent.isStopped = false;
        enemyBase2.SwitchState(state);
    }

}
