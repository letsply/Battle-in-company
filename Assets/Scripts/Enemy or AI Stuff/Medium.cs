using UnityEngine;

public class Medium : EnemyBase2
{
    public override void Start()
    {
        base.Start();
        attackType = AttackType.CloseRange;
    }
}
