using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.AI;

public abstract class StateMachineControler : MonoBehaviour, IModifieable
{
    float damageTime = 0;

    #region Components
    protected Rigidbody rb { get => gameObject.GetComponent<Rigidbody>(); }
    [SerializeField] protected GameObject itemHolder;
    [SerializeField] protected Animator itemHolderAnim;
    [SerializeField] protected LayerMask itemLayer;

    protected NavMeshAgent agent { get => GetComponent<NavMeshAgent>(); }
    public NavMeshAgent Agent() => agent;

    #endregion

    #region Values

    // when adding a new Value put it in the enum

    protected bool hitting = false;

    protected float grabbingRange = 2;
    [SerializeField] protected float bestWeight;
    public float BestWeight() => bestWeight;
    public GameObject ItemHolding() => itemHolding;

    [SerializeField] protected float attackRange;
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

    #region Get and set
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
        range
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
        "attackRange"
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

    #endregion

    #region StateMachine
    protected Transform attackingTarget;

    protected Vector3 destination;
    protected GameObject itemHolding;
    protected BaseState currentState;
    public enum States
    {
        Idle,
        SearchForItem,
        SearchPlayer,
        Attack
    }

    [SerializeField] private AttackType attackType;
    public enum AttackType
    {
        LongRange,
        CloseRange
    }
    public AttackType GetAttackType() => attackType;
    [SerializeField] private StratagieType stratagieType;
    public enum StratagieType
    {
        Aggresive,
        Devensive,
        Strategic
    }
    public StratagieType GetStratagie() => stratagieType;

    protected List<BaseState> baseStates = new List<BaseState>();

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


    public virtual void Start()
    {
        ApplyModifiers();

        Idle idle = new Idle();
        SearchForItem item = new SearchForItem();
        SearchPlayer player = new SearchPlayer();
        SearchPlayer attack = new SearchPlayer();

        baseStates.Add(idle);
        baseStates.Add(item);
        baseStates.Add(player);
        baseStates.Add(attack);

        SwitchState(States.Idle);
    }

    public virtual void Update()
    {
        if (currentState != null)
        {
            currentState.OnUpdate();
        }

        if (damageTime > 0)
        {
            damageTime -= Time.deltaTime;
        }

        if(itemHolding != null)
        {
            itemHolding.transform.position = itemHolder.transform.position;
        }
    }

    public virtual void FixedUpdate()
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


    #region actions
    public void Take()
    {
        Collider[] hitColliders = Physics.OverlapBox(itemHolder.transform.position, new Vector3(grabbingRange, GetComponent<CapsuleCollider>().height, grabbingRange), Quaternion.identity, itemLayer);
        // first collider in array gets used
        itemHolding = hitColliders[0].transform.gameObject;
        itemHolding.transform.parent = itemHolder.transform;
        itemHolding.transform.position = itemHolder.transform.position;
        itemHolding.transform.rotation = itemHolder.transform.rotation;

        itemHolding.GetComponent<Rigidbody>().useGravity = false;
        itemHolding.GetComponent<Rigidbody>().freezeRotation = true;
        itemHolding.GetComponent<Collider>().enabled = false;

        if (itemHolding.TryGetComponent<Item>(out Item item))
        {
            item.EnemyHolding(this);
        }
    }

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

    public void SwitchState(States state)
    {
        currentState = baseStates[(int)state];
        currentState.StartState(this);
    }

    public void SetDestination(Transform newDestination, bool isAttackingTarget)
    {
        destination = newDestination.position;

        if (isAttackingTarget)
        {
            attackingTarget = newDestination;
        }
        agent.destination = destination;
    }

    public void TakeDmg(float dmg)
    {
        if (damageTime <= 0)
        {
            health -= dmg;
            damageTime = 0.5f;
        }
    }
}
