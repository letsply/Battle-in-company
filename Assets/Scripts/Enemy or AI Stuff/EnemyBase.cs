using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EnemyBase : MonoBehaviour, IModifieable
{
    #region Values
    [SerializeField] private float strenght;
    [SerializeField] private float health;
    [SerializeField] private float criticalDamageMultiplier;

    #region Movement(Walking)

    [Header("MovementWalkValues")]
    [SerializeField] private float speed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRegain;
    [SerializeField] private bool infinitStamina;
    private float stamina;

    #endregion

    #region Movement(Jump)

    [Header("MovementWalkValues")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int jumps; // The amount off Jumps the player has
    [SerializeField] private float airResistance;

    private int maxJumps;
    #endregion

    public enum Values
    {
        strength,
        health,
        criticalDamageMultiplier,
        speed,
        sprintingSpeed,
        stamina,
        staminaRegain,
        infinitStamina,
        jumpForce,
        jumps,
        airResistance,
    }
    private string[] valueNames = new string[]
    {
        "strength",
        "health",
        "criticalDamageMultiplier",
        "speed",
        "sprintingSpeed",
        "stamina",
        "staminaRegain",
        "infinitStamina",
        "jumpForce",
        "jumps",
        "airResistance",
    };

    public T GetValue<T>(Values valueName)
    {
        string name = valueNames[(int)valueName];
        var field = GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        return (T)field.GetValue(this);
    }

    public void SetValue<T>(Values valueName, T value)
    {
        var field = GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(this, value);
    }


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
