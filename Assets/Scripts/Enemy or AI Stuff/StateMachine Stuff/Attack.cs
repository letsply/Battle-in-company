using NUnit.Framework.Constraints;
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
        Debug.Log("tafuq");
        float distance = Vector3.Distance(SMC.Agent().destination, SMC.transform.position);
        if (attackType == StateMachineControler.AttackType.CloseRange)
        {
            if (stratagie == StateMachineControler.StratagieType.Strategic)
            {
                if (SMC.GetValue<float>(EnemyValues.attackRange) > distance)
                {
                    SMC.Agent().isStopped = true;
                    SMC.Hit();
                    EndState();
                }
                else
                {
                    SMC.Agent().destination = SMC.GetValue<Transform>(EnemyValues.attackingTarget).position;
                }
            }
        }
        else
        {
            if (stratagie == StateMachineControler.StratagieType.Strategic)
            {
                if (SMC.GetValue<float>(EnemyValues.attackRange) > distance)
                {
                    bool playerInView = false;
                    List<Ray> view = new List<Ray>();
                    view.Clear();
                    for (int i = -1 * SMC.GetValue<int>(EnemyValues.viewAngle); SMC.GetValue<int>(EnemyValues.viewAngle) > i; i++)
                    {
                        Vector3 vector = SMC.transform.TransformDirection(Vector3.forward);
                        Vector3 dir = Quaternion.AngleAxis(i, SMC.transform.up) * vector;

                        Debug.DrawRay(SMC.transform.position, dir * SMC.GetValue<float>(EnemyValues.attackRange), Color.yellow);
                        Ray ray = new Ray(SMC.transform.position, dir);
                        view.Add(ray);
                    }

                    RaycastHit hit;
                    foreach (Ray ray in view)
                    {
                        if (Physics.Raycast(ray, out hit, SMC.GetValue<float>(EnemyValues.attackRange)))
                        {
                            if (hit.collider.tag == "Player")
                            {
                                playerInView = true;
                                Debug.Log("View");
                            }
                        }
                    }

                    if (playerInView)
                    {

                        RaycastHit Throwhit;
                        Ray ThrowRay = new Ray(SMC.transform.position, SMC.transform.TransformDirection(Vector3.forward));

                        if (Physics.Raycast(ThrowRay, out Throwhit, SMC.GetValue<float>(EnemyValues.attackRange)))
                        {
                            if (Throwhit.collider.tag == "Player")
                            {
                                SMC.Throw();
                                EndState();
                            }
                        }

                    }
                    else
                    {
                        SMC.Agent().destination = SMC.GetValue<Transform>(EnemyValues.attackingTarget).position;
                    }

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
