using UnityEngine;

public class ItemBaseModifier : BaseModifier
{
    protected Item item;

    public virtual void FindPlayer(Item itemToMod)
    {
        item = itemToMod;
    }
}
