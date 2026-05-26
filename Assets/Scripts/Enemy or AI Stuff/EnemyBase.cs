using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class EnemyBase : StateMachineControler, IModifieable
{
    float damageTime = 0;

    #region Components
    protected Rigidbody rb { get => gameObject.GetComponent<Rigidbody>(); }
    [SerializeField] private GameObject itemHolder;
    [SerializeField] private Animator itemHolderAnim;

    #endregion

    #region Values
    bool hitting = false;

    [SerializeField] protected float strength;
    [SerializeField] protected float health;
    [SerializeField] protected float criticalDamageMultiplier;

    #region Movement(Walking)

    [Header("MovementWalkValues")]
    [SerializeField] protected float speed;
    [SerializeField] protected float sprintingSpeed;
    [SerializeField] protected float maxStamina;
    [SerializeField] protected float staminaRegain;
    [SerializeField] protected bool infinitStamina;
    protected float stamina;

    #endregion

    #region Movement(Jump)

    [Header("MovementWalkValues")]
    [SerializeField] protected float jumpForce;
    [SerializeField] protected int jumps; // The amount off Jumps the player has
    [SerializeField] protected float airResistance;

    protected int maxJumps;
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
    protected string[] valueNames = new string[]
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
    protected List<AIBaseModifier> enemyModifiers = new List<AIBaseModifier>();
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

    protected List<BaseEffect> enemyEffects = new List<BaseEffect>();
    public void AddEffect(BaseEffect effect)
    {
        enemyEffects.Add(effect);
    }

    void Start()
    {
        ApplyModifiers();
    }

    void Update()
    {
        if (damageTime > 0)
        {
            damageTime -= Time.deltaTime;
        }
    }
    void FixedUpdate()
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
        if (damageTime <= 0)
        {
            health -= dmg;
            damageTime = 0.5f;
        }
    }

    #region Actions
    public void Hit()
    {
        if (itemHolding.TryGetComponent<Item>(out Item item) && hitting == false)
        {
            StartCoroutine(HitAnimate(item));
        }
    }

    public IEnumerator HitAnimate(Item item)
    {
        itemHolderAnim.SetTrigger("Punching");

        hitting = true;
        item.IsPunching(true);

        // Wait until the animation state is fully played (normalizedTime >= 1)
        yield return new WaitForSeconds(itemHolderAnim.GetCurrentAnimatorStateInfo(0).length);

        // Animation has finished
        item.IsPunching(false);
        hitting = false;
    }

    public void Throw()
    {

    }

    public void Use()
    {

    }
    #endregion

}
