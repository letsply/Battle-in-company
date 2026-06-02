
using System;
using System.Collections.Generic;

public class ItemBaseModifier : BaseModifier
{
    protected Item item;
    protected List<BaseEffect> effects = new List<BaseEffect>();
    protected bool hasUse;
    public bool HasUse() => hasUse;

    public void FindItem(Item itemToMod)
    {
        item = itemToMod;
    }

    public virtual void OnItemPunchHit(ref float damageMod,ref float critMod)
    {

    }

    public virtual void OnItemHit(ref float damageMod)
    {

    }

    public virtual void OnKnockback(ref float KnockbackMod)
    {

    }

    public virtual void OnItemUse()
    {

    }
    public void GiveEffect(Player player, StateMachineControler enemy)
    {
        if (effects != null && enemy == null)
        {
            foreach (BaseEffect effect in effects)
            {
                player.AddEffect(effect);
            }
        }
        else if(effects != null && player == null)
        {
            foreach (BaseEffect effect in effects)
            {
                enemy.AddEffect(effect);
            }
        }
    }
}
