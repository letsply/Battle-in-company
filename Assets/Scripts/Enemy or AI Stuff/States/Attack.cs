using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BaseState
{
    private EnemyBase2.AttackType attackType;
    private EnemyBase2.StratagieType stratagie;
    private Coroutine wait;
    private bool isFleeing;

    protected override void StartOverride()
    {
        attackType = enemyBase2.GetAttackType();
        stratagie = enemyBase2.GetStratagie();

        wait = null;
        isFleeing = false;
    }

    protected override void UpdateOverride()
    {
        if (attackType == EnemyBase2.AttackType.CloseRange)
        { CloseRangeCombat(); }
        else
        { LongRangeCombat(); }
    }

    void CloseRangeCombat()
    {
        if (stratagie == EnemyBase2.StratagieType.Strategic)
        {
            // View at Player
            Vector3 direction = enemyBase2.GetValue<GameObject>(EnemyValues.targetEnemy).transform.position - enemyBase2.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            if (enemyBase2.TargetIsAttackable())
            {
                 enemyBase2.Hit();
                 Flee();
            }
            else if(isFleeing == false)
            {
                enemyBase2.Agent.destination = enemyBase2.GetValue<GameObject>(EnemyValues.targetEnemy).transform.position;
            }
            else if (isFleeing)
            {
                Flee();
            }
        }
    }

    void LongRangeCombat()
    {
        if (stratagie == EnemyBase2.StratagieType.Strategic)
        {
            // View at Player
            Vector3 direction = enemyBase2.GetValue<GameObject>(EnemyValues.targetEnemy).transform.position - enemyBase2.transform.position;
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            enemyBase2.transform.rotation = Quaternion.RotateTowards(enemyBase2.transform.rotation, targetRotation, enemyBase2.Agent.angularSpeed * Time.deltaTime);

            if (enemyBase2.TargetIsAttackable())
            {
                enemyBase2.Agent.isStopped = true;

                RaycastHit Throwhit;
                Ray ThrowRay = new Ray(enemyBase2.transform.position, enemyBase2.transform.TransformDirection(Vector3.forward));

                if (Physics.Raycast(ThrowRay, out Throwhit, enemyBase2.GetValue<float>(EnemyValues.attackRange)))
                {
                    if (Throwhit.collider.tag == "Player")
                    {
                        if (wait == null)
                        {
                            enemyBase2.Throw();
                            wait = enemyBase2.StartCoroutine(Wait(enemyBase2.ItemHolderAnim.GetCurrentAnimatorStateInfo(0).length, wait));
                        }
                    }
                    else if (isFleeing == false)
                    {
                        enemyBase2.Agent.destination = enemyBase2.GetValue<GameObject>(EnemyValues.targetEnemy).transform.position;
                    }
                }

            }
            if (enemyBase2.HasItem() == false)
            {
                Flee();
            }

        }
    }

    public void Flee()
    {
        isFleeing = true;
        enemyBase2.Agent.isStopped = false;

        if (enemyBase2.TargetIsAttackable())
        {
            WalkAround();
        }
        if (enemyBase2.Agent.remainingDistance < 0.5f)
        {
            EndState(EnemyBase2.States.Idle);
        }
    }

    public void WalkAround()
    {
        // Generate a random destination to walk to in the range of 10
        Vector3 destination = new Vector3
        (
            enemyBase2.transform.position.x + Random.Range(-10, 10),
            enemyBase2.transform.position.y,
            enemyBase2.transform.position.z + Random.Range(-10, 10)
        );
        enemyBase2.Agent.destination = destination;

        // Player Evasion
        if (enemyBase2.TargetsInView().Count > 0)
        {
            Vector3 directionToTarget = enemyBase2.TargetsInView()[0].transform.position - enemyBase2.transform.position;
            enemyBase2.Agent.velocity = Vector3.Lerp(
                enemyBase2.Agent.desiredVelocity,
                -directionToTarget.normalized * enemyBase2.GetValue<float>(EnemyValues.sprintingSpeed),
                // The Distance when its completly away minus the actual distance diveded by the distance when its completly away
                Mathf.Clamp01(5 - directionToTarget.magnitude / 5)
            );
        }
    }


    protected override void EndState(EnemyBase2.States state)
    {
        enemyBase2.Agent.isStopped = false;
        enemyBase2.SwitchState(state);
    }


}
