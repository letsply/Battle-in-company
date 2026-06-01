using System.Collections.Generic;
using UnityEngine;

public class SearchPlayer : BaseState
{

    private List<Player> players = new List<Player>();
    private static int nearestPlayer(Player x, Player y,Vector3 stateMachine)
    {
        if (x == null)
        {
            if (y == null)
            { return 0; }
            else
            { return -1; }
        }
        else
        {
            if (y == null)
            { return 1; }
            else
            {
                float dist1 = Vector3.Distance(stateMachine, x.transform.position);
                float dist2 = Vector3.Distance(stateMachine, y.transform.position);
                return dist1.CompareTo(dist2);
            }
        }
    }

    protected override void StartOverride()
    {
        Search();
    }

    private void Search()
    {
        players.Clear();
        foreach (var player in MonoBehaviour.FindObjectsByType<Player>(FindObjectsSortMode.None))
        {
             players.Add(player);
        }
        if (players.Count == 0)
        {
            SMC.SwitchState(StateMachineControler.States.Idle);
        }
        else
        {
            if (players.Count >= 2)
            {
                players.Sort((x, y) => nearestPlayer(x, y, SMC.transform.position));
            }
            Transform destination = players[0].GetComponent<Transform>() as Transform;
            SMC.SetDestination(destination, true);

            // SMC.SwitchState(StateMachineControler.States.Attack);
        }
    }
    
}
