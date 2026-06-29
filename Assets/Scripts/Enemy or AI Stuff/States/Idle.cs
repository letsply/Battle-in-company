using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;

public class Idle : BaseState
{
    protected override void StartOverride()
    {
        if (enemyBase2.HasItem())
        {
            Debug.Log("SearchPlayer ...");
            enemyBase2.SwitchState(EnemyBase2.States.SearchPlayer); 
        }
        else if(GameObject.FindObjectsByType<Item>(FindObjectsSortMode.None).Length > 0)
        {
            Debug.Log("SearchItem ...");
            enemyBase2.SwitchState(EnemyBase2.States.SearchForItem); 
        }

        if (GameObject.FindObjectsByType<Item>(FindObjectsSortMode.None).Length == 0)
        {
            Debug.Log("NoItemsThere ...");
            WalkAround();
        }
    }

    protected override void UpdateOverride()
    {
        if (GameObject.FindObjectsByType<Item>(FindObjectsSortMode.None).Length == 0)
        {
            WalkAround();
        }
    }

    public void WalkAround()
    {

        // Generate a random destination to walk to in the range of 10
        if (enemyBase2.Agent.velocity.magnitude <= 0.05f || enemyBase2.Agent.remainingDistance < enemyBase2.Agent.stoppingDistance || waiting == false)
        {
            Vector3 destination = new Vector3
            (
                enemyBase2.transform.position.x + Random.Range(-10, 10),
                enemyBase2.transform.position.y,
                enemyBase2.transform.position.z + Random.Range(-10, 10)
            );
            enemyBase2.Agent.destination = destination;
        }

        // If agent takes longer then anticipatet search new dest
        if (waiting == false)
        {
            float time = enemyBase2.Agent.desiredVelocity.magnitude / enemyBase2.Agent.remainingDistance;
            // plus a buffer second
            if (time != float.NaN)
            { enemyBase2.StartCoroutine(Wait(time + 1)); }
        }

        Evade();

    }

    public void Evade()
    {
        if (enemyBase2.TargetsInView().Count > 0)
        {
            // Player Evasion
            float playerDistanceToPoint = Vector3.Distance(enemyBase2.Agent.destination, enemyBase2.TargetsInView()[0].transform.position);
            float enemyDistanceToPoint = Vector3.Distance(enemyBase2.Agent.destination, enemyBase2.transform.position);

            if (playerDistanceToPoint > enemyDistanceToPoint && waiting)
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
