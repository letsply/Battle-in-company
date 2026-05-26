using System.Collections.Generic;
using UnityEngine;

public abstract class StateMachineControler : MonoBehaviour
{
    protected BaseState currentState;
    protected GameObject itemHolding;
    protected float bestWeight;

    public float BestWeight() => bestWeight;
    public GameObject ItemHolding() => itemHolding;

    public enum States
    {
        Idle,
        SearchForItem,
        SearchPlayer
    }

    protected List<BaseState> baseStates = new List<BaseState>();

    private void Start()
    {
        Idle idle = new Idle();
        SearchForItem item = new SearchForItem();
        SearchPlayer player = new SearchPlayer();

        baseStates.Add(idle);
        baseStates.Add(item);
        baseStates.Add(player);
    }

    void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }
    }

    public void SwitchState(States state)
    {
        currentState = baseStates[(int)state];
        currentState.StartState(this);
    }
   

}
