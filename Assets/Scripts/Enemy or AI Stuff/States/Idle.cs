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
            enemyBase2.SwitchState(EnemyBase2.States.SearchForItem); 
        }

        if (GameObject.FindObjectsByType<Item>(FindObjectsSortMode.None).Length == 0)
        {
            WalkAround();
        }
    }

    public void WalkAround()
    {
        // Generate a random destination to walk to in the range of 10
        if (enemyBase2.Agent.velocity.magnitude <= 0.0125f)
        {
            Vector3 destination = new Vector3
            (
                enemyBase2.transform.position.x + Random.Range(-10, 10),
                enemyBase2.transform.position.y,
                enemyBase2.transform.position.z + Random.Range(-10, 10)
            );
            enemyBase2.Agent.destination = destination;
        }

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

        if (GameObject.FindObjectsByType<Item>(FindObjectsSortMode.None).Length == 0)
        {
            WalkAround();
        }
    }
}
