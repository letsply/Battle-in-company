using UnityEngine;

public class Idle : BaseState
{
    protected override void StartOverride()
    {
        if (MonoBehaviour.FindAnyObjectByType<Item>() != null && MonoBehaviour.FindAnyObjectByType<Player>() != null)
        {
            if (SMC.ItemHolding() == null)
            {
                Debug.Log("Searching for Item ...");
                SMC.SwitchState(StateMachineControler.States.SearchForItem);
            }
            else
            {
                if (SMC.GetValue<Transform>(EnemyValues.attackingTarget) == null)
                {
                    Debug.Log("Searching for Player ...");
                    SMC.SwitchState(StateMachineControler.States.SearchPlayer);
                }
                else
                {
                    Debug.Log("Attacking ... ");
                    SMC.SwitchState(StateMachineControler.States.Attack);
                }
            }
        }
    }

}
