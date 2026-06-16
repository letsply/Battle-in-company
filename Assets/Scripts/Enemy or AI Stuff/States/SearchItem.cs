using System.Collections.Generic;
using UnityEngine;

public class SearchItem : BaseState
{
    private List<Item> items = new List<Item>();

    private float diseredWeight { get => enemyBase2.GetValue<float>(EnemyValues.perfectWeight); }
    private static int nearestWeight(float diseredWeight, Item x, Item y)
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
        if (items.Count == 0)
        {
            
        }
        else
        {
            items.Sort((x, y) => nearestWeight(diseredWeight, x, y));
            enemyBase2.SetDestination(items[0].gameObject, false);
        }
    }

    protected override void UpdateOverride()
    {

        if (enemyBase2.ItemHeld() == null)
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
}
