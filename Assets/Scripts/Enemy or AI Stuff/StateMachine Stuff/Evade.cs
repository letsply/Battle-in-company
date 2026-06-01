using UnityEngine;

public class Evade : BaseState
{
    Transform PlayerPos;

    protected override void StartOverride()
    {
        PlayerPos = SMC.GetValue<Transform>(StateMachineControler.Values.attackingTarget);

        float PlayerZoneX = PlayerPos.position.x + 5;
        float PlayerZoneY = PlayerPos.position.y + 5;
        float PlayerZoneZ = PlayerPos.position.z + 5;

        Random.insideUnitSphere

        float RandomX = Random.Range(PlayerZoneX,PlayerZoneX);
        float RandomY = Random.Range(PlayerZoneX, PlayerZoneX);
        float RandomZ = Random.Range(PlayerZoneX, PlayerZoneX);
        Vector3 RandomPlayerEvasionPoint = new Vector3();

        SMC.SetDestination();
    }

    
}
