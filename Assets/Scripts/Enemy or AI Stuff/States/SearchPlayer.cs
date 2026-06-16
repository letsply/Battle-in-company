using UnityEngine;

public class SearchPlayer : BaseState
{
    protected override void StartOverride()
    {
        WalkAround();
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

        if (enemyBase2.TargetsInView().Count > 0)
        {
            enemyBase2.SetDestination(enemyBase2.TargetsInView()[0],true);
            enemyBase2.SwitchState(EnemyBase2.States.Attack);
        }

        if (GameObject.FindObjectsByType<Item>(FindObjectsSortMode.None).Length == 0)
        {
            WalkAround();
        }
    }
}
