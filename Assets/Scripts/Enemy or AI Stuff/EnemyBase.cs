using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EnemyBase : MonoBehaviour, IModifieable
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

    public float SetValue(ref float oldValue, float newValue) => oldValue = newValue;

    #endregion

    #region ModifierStuff
    private List<AIBaseModifier> enemyModifiers = new List<AIBaseModifier>();
    public void ApplyModifiers()
    {
        if (enemyModifiers.Count > 0)
        {
            foreach (var mod in enemyModifiers)
            {
                mod.Modify();
                mod.FindEnemy(this);
            }
        }
    }

    #endregion

    private List<BaseEffect> enemyEffects = new List<BaseEffect>();
    public void AddEffect(BaseEffect effect)
    {
        enemyEffects.Add(effect);
    }

    public void Start()
    {
        ApplyModifiers();
    }

    public void FixedUpdate()
    {
        for (int i = 0; i < enemyEffects.Count; i++)
        {
            enemyEffects[i].Update();
            if (enemyEffects[i].GetDuration() <= 0)
            {
                enemyEffects.RemoveAt(i);
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

}
