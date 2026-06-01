using UnityEngine;

public class Idle : BaseState
{
    protected override void StartOverride()
    {
        if (MonoBehaviour.FindAnyObjectByType<Item>() != null && MonoBehaviour.FindAnyObjectByType<Player>() != null)
        {
            if (SMC.ItemHolding() == null)
            {
                SMC.SwitchState(StateMachineControler.States.SearchForItem);
            }
            else
            {
                if (SMC.GetValue<Transform>(StateMachineControler.Values.attackingTarget) == null)
                {
                    SMC.SwitchState(StateMachineControler.States.SearchPlayer);
                }
                else
                {
                    SMC.SwitchState(StateMachineControler.States.Attack);
                }
            }
        }
    }

}
