using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

public class SearchItem : BaseState
{
    private List<Item> items = new List<Item>();

    private float diseredWeight { get => enemyBase2.GetValue<float>(EnemyValues.perfectWeight); }
    private static int sortByWeight(float diseredWeight, Item x, Item y)
    {
        if (x == null)
        {

            if (y == null)
            { return 0; }
            else
            { return -1; }
        }

        else
        {

            if (y == null)
            { return 1; }
            else
            {
                float dist1 = Mathf.Abs(diseredWeight - x.ItemsData().Weight());
                float dist2 = Mathf.Abs(diseredWeight - y.ItemsData().Weight());
                return dist1.CompareTo(dist2);
            }
        }
    }
    private static int sortByDistance(Vector3 enemyBase2Pos, Item x, Item y)
    {
        if (x == null)
        {

            if (y == null)
            { return 0; }
            else
            { return -1; }
        }

        else
        {

            if (y == null)
            { return 1; }
            else
            {
                float dist1 = Vector3.Distance(enemyBase2Pos,x.transform.position);
                float dist2 = Vector3.Distance(enemyBase2Pos, y.transform.position);
                return dist1.CompareTo(dist2);
            }
        }
    }

    protected override void StartOverride()
    {
        Search();
    }

    private void Search()
    {
        items.Clear();
        foreach (var item in MonoBehaviour.FindObjectsByType<Item>(FindObjectsSortMode.None))
        {
            if (item.ItemIsHeld() == false)
            {
                items.Add(item);
            }
        }
        if(items.Count != 0)
        {
            items.Sort((x, y) => sortByWeight(diseredWeight, x, y));
            items.Sort((x, y) => sortByDistance(enemyBase2.transform.position, x, y));
            enemyBase2.SetDestination(items[0].gameObject, false);
        }
    }

    protected override void UpdateOverride()
    {
        // Evade Player
        if (enemyBase2.TargetsInView().Count > 0)
        {
            Evade();
        }

        if (enemyBase2.HasItem() == false)
        {
            if (items[0].ItemIsHeld())
            {
                Search();
            }

            if (enemyBase2.Agent.remainingDistance <= 2)
            {
                enemyBase2.Take();
            }
        }
        else
        {
            EndState(EnemyBase2.States.Idle);
        }

    }

    private void Evade()
    {
        // Player Evasion
        float playerDistanceToPoint = Vector3.Distance(enemyBase2.Agent.destination, enemyBase2.TargetEnemy().transform.position);
        float enemyDistanceToPoint = Vector3.Distance(enemyBase2.Agent.destination, enemyBase2.transform.position);

        if (playerDistanceToPoint > enemyDistanceToPoint)
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
