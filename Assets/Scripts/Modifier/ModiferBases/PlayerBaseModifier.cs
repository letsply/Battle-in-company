using UnityEngine;

public class PlayerBaseModifier : BaseModifer
{
    protected Player player;

    public virtual void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    } 
}
