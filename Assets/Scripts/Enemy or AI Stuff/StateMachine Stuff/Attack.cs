using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Attack : BaseState
{
    StateMachineControler.AttackType attackType;
    StateMachineControler.StratagieType stratagie;

    protected override void StartOverride()
    {
        attackType = SMC.GetAttackType();

        stratagie = SMC.GetStratagie();
    }

    protected override void UpdateOverride()
    {
        float distance = Vector3.Distance(SMC.Agent().destination, SMC.transform.position);
        if (attackType == StateMachineControler.AttackType.CloseRange)
        {
            if (stratagie == StateMachineControler.StratagieType.Strategic)
            {
                if (SMC.GetValue<float>(StateMachineControler.Values.range) > distance)
                {
                    SMC.Agent().stoppingDistance = SMC.GetValue<float>(StateMachineControler.Values.range);
                    SMC.Hit();
                    EndState();
                }
                else
                {
                    SMC.Agent().destination = SMC.GetValue<Transform>(StateMachineControler.Values.attackingTarget).position;
                    SMC.Agent().stoppingDistance = 1;
                }
            }
        }
        else
        {
            if (SMC.GetValue<float>(StateMachineControler.Values.range) > distance)
            {

            }

        }
    }

    protected override void EndState()
    {
        SMC.SwitchState(StateMachineControler.States.Evade);
    }


}
