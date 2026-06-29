using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : BaseState
{
    private EnemyBase2.AttackType attackType;
    private EnemyBase2.StratagieType stratagie;
    private bool isFleeing;

    protected override void StartOverride()
    {
        attackType = enemyBase2.GetAttackType();
        stratagie = enemyBase2.GetStratagie();

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
            if (enemyBase2.TargetIsAttackable() )
            {
                 enemyBase2.Hit();
                 Flee();
            }
            else if(isFleeing == false)
            {
                enemyBase2.Agent.destination = enemyBase2.GetValue<GameObject>(EnemyValues.targetEnemy).transform.position;
            }
            if (isFleeing)
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

            if (enemyBase2.TargetIsAttackable() && isFleeing == false)
            {
                RaycastHit Throwhit;
                Ray ThrowRay = new Ray(enemyBase2.transform.position, enemyBase2.transform.TransformDirection(Vector3.forward));

                if (Physics.Raycast(ThrowRay, out Throwhit, enemyBase2.GetValue<float>(EnemyValues.attackRange)))
                {
                    if (Throwhit.collider.tag == "Player")
                    {
                        enemyBase2.Agent.isStopped = true;
                        if (waiting == false && enemyBase2.HasItem() == true)
                        {
                            enemyBase2.Throw();
                            enemyBase2.StartCoroutine(Wait(enemyBase2.ItemHolderAnim.GetCurrentAnimatorStateInfo(0).length + 0.15f));
                        }
                    }
                }

            }
            if (enemyBase2.HasItem() == false || isFleeing)
            {
                Flee();
            }

        }
    }

    public void Flee()
    {
        // First Time Flee Gets Called
        if(isFleeing == false)
        {
            enemyBase2.StartCoroutine(Wait(3));
            isFleeing = true;
            enemyBase2.Agent.isStopped = false;
        }
            
        WalkAround();

        if (enemyBase2.Agent.remainingDistance < enemyBase2.Agent.stoppingDistance)
        {
            isFleeing = false;
            EndState(EnemyBase2.States.Idle);
        }
    }

    public void WalkAround()
    {
        // Generate a random destination to walk to in the range of 10

        if (enemyBase2.Agent.remainingDistance < enemyBase2.Agent.stoppingDistance)
        {
            Vector3 destination = new Vector3
            (
            enemyBase2.transform.position.x + Random.Range(-10, 10),
            enemyBase2.transform.position.y,
            enemyBase2.transform.position.z + Random.Range(-10, 10)
            );
            enemyBase2.Agent.destination = destination;
        }

        Evade();

    }
    private void Evade()
    {
        // Player Evasion
        float playerDistanceToPoint = Vector3.Distance(enemyBase2.Agent.destination, enemyBase2.TargetEnemy().transform.position);
        float enemyDistanceToPoint = Vector3.Distance(enemyBase2.Agent.destination, enemyBase2.transform.position);

        if (playerDistanceToPoint > enemyDistanceToPoint && waiting == true)
        {
            if (enemyBase2.TargetsInView().Count > 0)
            {
                Vector3 directionToTarget = enemyBase2.TargetsInView()[0].transform.position - enemyBase2.transform.position;
                enemyBase2.Agent.velocity = Vector3.Lerp(
                    enemyBase2.Agent.desiredVelocity,
                    -directionToTarget.normalized * enemyBase2.GetValue<float>(EnemyValues.sprintingSpeed),
                    // The Distance when its completly away minus the actual distance diveded by the distance when its completly away
                    Mathf.Clamp01(2 - directionToTarget.magnitude / 4)
                );
            }
        }
    }

}
