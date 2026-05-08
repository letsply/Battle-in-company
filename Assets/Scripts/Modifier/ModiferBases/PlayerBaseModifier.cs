using UnityEngine;

public abstract class PlayerBaseModifier : BaseModifier
{
    protected Player player;

    public virtual void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
    } 
}
