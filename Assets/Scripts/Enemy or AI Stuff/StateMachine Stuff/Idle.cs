using UnityEngine;

public class Idle : BaseState
{
    protected override void StartOverride()
    {
        if (FindAnyObjectByType(typeof(Item)) != null)
        {
            if (SMC.ItemHolding() == null)
            {
                SMC.SwitchState(StateMachineControler.States.SearchForItem);
            }
            else
            {
                SMC.SwitchState(StateMachineControler.States.SearchPlayer);
            }
        }
    }

}
