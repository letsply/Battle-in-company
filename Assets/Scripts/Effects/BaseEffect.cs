using System.Collections.Generic;
using UnityEngine;

public abstract class BaseEffect
{
    Player player;
    StateMachineControler enemy;

    public void FindPlayer(Player playerToEffect)
    {
        player = playerToEffect;
    }

    public void FindEnemy(StateMachineControler enemyToEffect)
    {
        enemy = enemyToEffect;
    }

    protected float duration;
    public float GetDuration() => duration;

    protected float strenght;

    protected float ticklenght;
    protected float timeScinceLastTick;

    protected bool doEffect = true;
    protected float valueBeforModifcation;
    protected enum EffectApplyanceType
    {
        OneTime,
        Tickbased, // every tick (ticks can have different lenghts)
        Update, // Apply for everytime Update gets called
        InDuration // Apply effect and remove it when duration = 0;
    }
    protected EffectApplyanceType ApplyanceType;

    public void Update()
    {
        if (duration > 0)
        {
            switch (ApplyanceType)
            {
                case EffectApplyanceType.OneTime:
                    ApplyEffect();
                    duration = 0;
                    break;
                case EffectApplyanceType.Tickbased:
                    Tick();
                    break;
                case EffectApplyanceType.Update:
                    ApplyEffect();
                    break;
                case EffectApplyanceType.InDuration:
                    ApplyEffect();
                    break;
            }
        }
        else
        {
            RemoveEffect();
        }
       
    }

    public virtual void Effect()
    {
        // Do the desired effect like fire damage or so
    }

    public void Tick()
    {
        timeScinceLastTick += Time.deltaTime;
        if (timeScinceLastTick >= ticklenght)
        {
            ApplyEffect();
            timeScinceLastTick = 0;
        }
    }

    public void ApplyEffect()
    {
        if (ApplyanceType == EffectApplyanceType.InDuration)
        {   
            Effect();
            doEffect = false;
        }
        if (doEffect)
        { Effect(); }
    }

    public virtual void RemoveEffect()
    {
        // corect or change effected values in the end
    }
}
