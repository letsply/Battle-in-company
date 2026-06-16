using System.Collections.Generic;
using UnityEngine;

public class EnemyView 
{
    private List<GameObject> targetsInView = new List<GameObject>();
    private List<Ray> view = new List<Ray>();
    private bool targetIsAttackable;

    // Values from Base
    private EnemyBase2 enemyBase2;
    private float viewAngle;
    private float attackRange;

    // Values To base
    public List<GameObject> TargetsInView() => targetsInView;
    public bool TargetIsAttackable() => targetIsAttackable;

    public void GetEnemyBase(EnemyBase2 enemyBase)
    {
        viewAngle = enemyBase.GetValue<float>(EnemyValues.viewAngle);
        attackRange = enemyBase.GetValue<float>(EnemyValues.attackRange);

        enemyBase2 = enemyBase;
    }

    public void OnUpdate()
    {
        if (enemyBase2 != null)
        {
            // make Rays
            view.Clear();
            for (float i = -1f * viewAngle; viewAngle > i; i++)
            {
                Vector3 vector = enemyBase2.transform.TransformDirection(Vector3.forward);
                Vector3 dir = Quaternion.AngleAxis(i, enemyBase2.transform.up) * vector;

                //Debug.DrawRay(enemyBase2.transform.position, dir * enemyBase2.GetValue<float>(EnemyValues.attackRange), Color.yellow);
                Ray ray = new Ray(enemyBase2.transform.position, dir);
                view.Add(ray);
            }

            // gets Targets In View
            RaycastHit hit;
            targetsInView.Clear();
            foreach (Ray ray in view)
            {
                if (Physics.Raycast(ray, out hit) && hit.collider.gameObject.tag == "Player")
                {
                    targetsInView.Add(hit.collider.gameObject);
                }
            }
            // Gets targets in a range and adds them to the View list
            Vector3 size = new Vector3(5, 2, 5);
            Collider[] hitColliders = Physics.OverlapBox(enemyBase2.transform.position, size, Quaternion.identity);
            if (hitColliders.Length > 0)
            {
                foreach (Collider collider in hitColliders)
                {
                    if (collider.gameObject.tag == "Player")
                    {
                        targetsInView.Add(collider.gameObject);
                    }
                }
            }

            if (enemyBase2.IsStateCurrent(EnemyBase2.States.Attack))
            {
                IsInAttackRange();
            }
        }
    }

    void IsInAttackRange()
    {
        // get Targets In Range Of Attacks
        RaycastHit hit;
        foreach (Ray ray in view)
        {
            if (Physics.Raycast(ray, out hit, attackRange))
            {
                if (hit.collider.gameObject == enemyBase2.TargetEnemy())
                {
                    targetIsAttackable = true;

                    break;
                }
                else
                {
                    targetIsAttackable = false;
                }
            }
        }
    }
}
