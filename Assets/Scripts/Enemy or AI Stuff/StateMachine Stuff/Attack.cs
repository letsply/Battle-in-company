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
            while (stratagie == StateMachineControler.StratagieType.Strategic)
            {
                if (SMC.GetValue<float>(StateMachineControler.Values.range) > distance)
                {

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

}
