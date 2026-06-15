using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class Evade : BaseState
{
    Transform playerPos;
    Vector3 fleeingPos = new Vector3();
    protected override void StartOverride()
    {
        playerPos = SMC.GetValue<Transform>(EnemyValues.attackingTarget);

        #region PlayerZoneX

        //the positive x positions where the enemy can flee
        float playerZoneXMin = 0;
        float playerZoneXMax = 0;
        //the positive x negative where the enemy can flee
        float nPlayerZoneXMin = 0;
        float nPlayerZoneXMax = 0;

        int i = (SMC.ItemHolding() == null) ? i = 15 : i = 30;

        for (; i >= 0; i--)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(playerPos.position, out hit, playerPos.position.x + i, NavMesh.AllAreas))
            {
                playerZoneXMax = playerPos.position.x + i;
                playerZoneXMin = playerPos.position.x + i / 5;
                break;
            }
        }

        i = (SMC.ItemHolding() == null) ? i = -15 : i = -30;

        for (; i <= 0; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(playerPos.position, out hit, playerPos.position.x + i, NavMesh.AllAreas))
            {
                nPlayerZoneXMax = playerPos.position.x + i;
                nPlayerZoneXMin = playerPos.position.x + i / 5;

                break;
            }
        }

        float randomX = Random.Range(playerZoneXMin, playerZoneXMax);
        float nRandomX = Random.Range(nPlayerZoneXMax, nPlayerZoneXMin);

        int xNegativeOrPositive = Random.Range(0,2);

        float xFleeingPos = 0;
        if (xNegativeOrPositive == 0)
        { xFleeingPos = randomX; }
        else
        { xFleeingPos = nRandomX; }

        #endregion

        #region PlayerZoneZ


        //the positive Z positions where the enemy can flee
        float playerZoneZMin = 0;
        float playerZoneZMax = 0;
        //the positive x negative where the enemy can flee
        float nPlayerZoneZMin = 0;
        float nPlayerZoneZMax = 0;

        i = (SMC.ItemHolding() == null) ? i = 15 : i = 30;

        for (; i >= 0; i--)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(playerPos.position, out hit, playerPos.position.z + i, NavMesh.AllAreas))
            {
                playerZoneZMax = playerPos.position.z + i;
                playerZoneZMin = playerPos.position.z + i / 5;

                break;
            }
        }

        i = (SMC.ItemHolding() == null) ? i = -15 : i = -30;

        for (; i <= 0; i++)
        {
            NavMeshHit hit;
            if (NavMesh.SamplePosition(playerPos.position, out hit, playerPos.position.z + i, NavMesh.AllAreas))
            {
                nPlayerZoneZMax = playerPos.position.z + i;
                nPlayerZoneZMin = playerPos.position.z + i / 5;

                break;
            }
        }

        float randomZ = Random.Range(playerZoneXMin, playerZoneXMax);
        float nRandomZ = Random.Range(nPlayerZoneXMax, nPlayerZoneXMin);

        int zNegativeOrPositive = Random.Range(0, 2);

        float zFleeingPos = 0;
        if (zNegativeOrPositive == 0)
        { zFleeingPos = randomZ; }
        else
        { zFleeingPos = nRandomZ; }


        #endregion

        fleeingPos = new Vector3(xFleeingPos, 0, zFleeingPos);

        SMC.Agent().destination = fleeingPos;
    }

    protected override void UpdateOverride()
    {
        if (SMC.Agent().velocity.magnitude < 0.15f && SMC.ItemHolding() != null)
        {
            if (SMC.ItemHolding().GetComponent<Item>().ItemIsUsable())
            {
                SMC.Use();
                EndState();
            }
        }
        else if (SMC.Agent().velocity.magnitude < 0.15f)
        {
            SMC.StartCoroutine(Wait());
            EndState();
        }
    }

    private IEnumerator Wait(float timeToWait = 2.5f)
    {
        yield return new WaitForSeconds(timeToWait);
    }
}
