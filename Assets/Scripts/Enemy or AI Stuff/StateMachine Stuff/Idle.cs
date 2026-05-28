using UnityEngine;

public class Idle : BaseState
{
    protected override void StartOverride()
    {
        if (MonoBehaviour.FindAnyObjectByType(typeof(Item)) != null && MonoBehaviour.FindAnyObjectByType(typeof(Player)) != null)
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
