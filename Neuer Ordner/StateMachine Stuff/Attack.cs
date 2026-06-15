using NUnit.Framework.Constraints;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.ShaderGraph.Internal.KeywordDependentCollection;

public class Attack : BaseState
{
    private StateMachineControler.AttackType attackType;
    private StateMachineControler.StratagieType stratagie;
    private Coroutine coroutine;
    private float distance;

    protected override void StartOverride()
    {
        attackType = SMC.GetAttackType();
        stratagie = SMC.GetStratagie();
    }

    protected override void UpdateOverride()
    {
        distance = Vector3.Distance(SMC.Agent().destination, SMC.transform.position);
        if (attackType == StateMachineControler.AttackType.CloseRange)
        { CloseRangeCombat(); }
        else
        { LongRangeCombat(); }
    }

    void CloseRangeCombat()
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

    void LongRangeCombat()
    {
        if (stratagie == StateMachineControler.StratagieType.Strategic)
        {

            bool playerInView = false;
            List<Ray> view = new List<Ray>();
            view.Clear();
            for (int i = -1 * SMC.GetValue<int>(EnemyValues.viewAngle); SMC.GetValue<int>(EnemyValues.viewAngle) > i; i++)
            {
                Vector3 vector = SMC.transform.TransformDirection(Vector3.forward);
                Vector3 dir = Quaternion.AngleAxis(i, SMC.transform.up) * vector;

                //Debug.DrawRay(SMC.transform.position, dir * SMC.GetValue<float>(EnemyValues.attackRange), Color.yellow);
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
                        SMC.Agent().speed = 0;

                        break;
                    }
                }
            }

            if (playerInView)
            {
                Quaternion.RotateTowards(SMC.transform.rotation, SMC.GetValue<Transform>(EnemyValues.attackingTarget).rotation, 10);

                RaycastHit Throwhit;
                Ray ThrowRay = new Ray(SMC.transform.position, SMC.transform.TransformDirection(Vector3.forward));

                if (Physics.Raycast(ThrowRay, out Throwhit, SMC.GetValue<float>(EnemyValues.attackRange)))
                {
                    if (Throwhit.collider.tag == "Player")
                    {
                        if (coroutine == null)
                        {
                            SMC.Throw();
                        }
                        coroutine = SMC.StartCoroutine(WaitForEndOfAction());
                    }
                }

            }

        }
    }

    public IEnumerator WaitForEndOfAction()
    {
        yield return new WaitForSeconds(SMC.GetValue<Animator>(EnemyValues.itemHolderAnim).GetCurrentAnimatorStateInfo(0).length);

        SMC.Agent().speed = SMC.GetValue<float>(EnemyValues.speed);
        EndState();
    }

    protected override void EndState()
    {
        SMC.Agent().isStopped = false;
        SMC.SwitchState(StateMachineControler.States.Evade);
    }


}
