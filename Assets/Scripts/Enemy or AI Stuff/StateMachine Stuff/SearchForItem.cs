using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
            EndState();
        }
        else
        {
            items.Sort((x, y) => nearestWeight(diseredWeight, x, y));
            SMC.SetDestination(items[0].GetComponent<Transform>(),false);
        }
    }

    protected override void UpdateOverride()
    {

        if (SMC.ItemHolding() == null)
        {
            if (items[0].ItemIsHeld())
            {
                Search();
            }

            if (SMC.Agent().remainingDistance <= 2)
            {
                SMC.Take();
            }
        }
        else
        {
            EndState();
        }
        
    }
}
