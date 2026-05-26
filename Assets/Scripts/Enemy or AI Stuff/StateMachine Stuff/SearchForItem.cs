using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SearchForItem : BaseState
{
    private List<Item> items = new List<Item>();

    private float diseredWeight { get => SMC.BestWeight(); }
    private static int nearestWeight(float diseredWeight ,Item x, Item y)
    {
        if (x == null) {

            if (y == null) 
            {  return 0;  }
            else 
            {  return -1; }
        }

        else {

            if (y == null) 
            {  return 1;  }
            else {
                float dist1 = Mathf.Abs(diseredWeight - x.ItemsData().Weight());
                float dist2 = Mathf.Abs(diseredWeight - y.ItemsData().Weight());
                return dist1.CompareTo(dist2);
            }
        }
    }

    protected override void StartOverride()
    {
        foreach (var item in FindObjectsByType<Item>(FindObjectsSortMode.None))
        {
            items.Add(item);
        }
        if (items.Count == 0)
        {
            SMC.SwitchState(StateMachineControler.States.Idle);
        }
        else
        {
            items.Sort((x, y) => nearestWeight(diseredWeight, x, y));
            items[0]
        }
    }

    protected override void UpdateOverride()
    {
        
    }
}
