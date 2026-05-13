using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IModifieable
{
    #region Values
    [SerializeField] private float strenght;
    [SerializeField] private float health;
    [SerializeField] private float speed;
    [SerializeField] private float criticalDamageMultiplier;

    public float GetStrenght() => strenght;
    public float GetHealth() => health;
    public float GetSpeed() => speed;
    public float GetCriticalDamageMultiplier() => criticalDamageMultiplier;

    public float SetValue(ref float oldValue,float newValue) => oldValue = newValue;

    #endregion

    #region ModifierStuff
    private List<PlayerBaseModifier> playerModifiers = new List<PlayerBaseModifier>();
    public void ApplyModifiers()
    {
        if (playerModifiers.Count > 0)
        {
            foreach (var mod in playerModifiers) {
                mod.Modify();
                mod.FindPlayer(this);
            }
        }
    }

    #endregion

    private List<BaseEffect> playerEffects = new List<BaseEffect>();
    public void AddEffect(BaseEffect effect)
    {
        playerEffects.Add(effect);
    }

    public void Start()
    {
        ApplyModifiers();
    }

    public void FixedUpdate()
    {
       for (int i = 0; i < playerEffects.Count; i++)
       {
            playerEffects[i].Update();
            if (playerEffects[i].GetDuration() <= 0)
            {
                playerEffects.RemoveAt(i);
            }
       }
    }

    public void TakeDmg(float dmg)
    {
         health -= dmg;
    }

    #region Actions
    private void Hit()
    {

    }

    private void Throw()
    {

    }

    private void Use()
    {

    }
    #endregion

    #region Input
    public void TakeItem(InputAction.CallbackContext context)
    {

    }

    public void HitInput(InputAction.CallbackContext context)
    {

    }

    public void ThrowInput(InputAction.CallbackContext context)
    {

    }
    #endregion

}

