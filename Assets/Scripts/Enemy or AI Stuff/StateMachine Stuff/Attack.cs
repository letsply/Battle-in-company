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
                    SMC.Agent().isStopped = true;
                    SMC.Hit();
                    EndState();
                }
                else
                {
                    SMC.Agent().destination = SMC.GetValue<Transform>(StateMachineControler.Values.attackingTarget).position;
                }
            }
        }
        else
        {
            if (stratagie == StateMachineControler.StratagieType.Strategic)
            {
                if (SMC.GetValue<float>(StateMachineControler.Values.range) > distance)
                {
                    SMC.Agent().isStopped = true;
                    SMC.Throw();
                    EndState();
                }
                else
                {
                    SMC.Agent().destination = SMC.GetValue<Transform>(StateMachineControler.Values.attackingTarget).position;
                }
            }

        }
    }

    protected override void EndState()
    {
        SMC.Agent().isStopped = false;
        SMC.SwitchState(StateMachineControler.States.Evade);
    }


}
