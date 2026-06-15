using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;

public enum EnemyValues
{
    strength,
    health,
    criticalDamageMultiplier,
    speed,
    sprintingSpeed,
    stamina,
    infinitStamina,
    jumpForce,
    jumps,
    airResistance,
    attackRange,
    targetEnemy,
    viewAngle,
    viewRange,
    itemHolderAnim
}


public class EnemyBase2 : MonoBehaviour
{
    #region Commponents

    [Header("Commponents & ChildObject stuff")]
    protected Rigidbody rb => gameObject.GetComponent<Rigidbody>();

    [SerializeField] protected GameObject itemHolder;
    protected Animator itemHolderAnim => itemHolder.GetComponent<Animator>(); 

    [SerializeField] protected GameObject itemHeld;
    [SerializeField] protected LayerMask itemLayer;

    protected NavMeshAgent agent => GetComponent<NavMeshAgent>();
    protected GameObject targetEnemy;
    protected Vector3 targetPos;

    #endregion

    #region Values

    float lastDamageTime = 0;
    protected bool hitting;
    protected float grabbingRange = 2;

    [Header("Non Movement Stuff")]
    [SerializeField] protected float viewAngle = 45;
    [SerializeField] protected float viewRange = 45;
    [SerializeField] protected float perfectWeight = 10;
    [SerializeField] protected float attackRange = 8;

    [SerializeField] protected float strenght = 1000;
    [SerializeField] protected float health = 100;
    [SerializeField] protected float criticalDamageMultiplier = 2;

    [Header("Movement Stuff")]
    [SerializeField] protected float speed = 3.5f;
    [SerializeField] protected float sprintingSpeed = 5;
    [SerializeField] protected float jumpForce;
    [SerializeField] protected float jumps;
    [SerializeField] protected float airResistance;

    #endregion

    #region Get and set
    public T GetValue<T>(EnemyValues valueName)
    {
        string name = valueName.ToString();
        var field = GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        return (T)field.GetValue(this);
    }

    public void SetValue<T>(EnemyValues valueName, T value)
    {
        var field = GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(this, value);
    }

    #endregion

    #region StateMachine

    protected BaseState currentState;
    protected List<BaseState> states = new List<BaseState>();

    public enum States
    {
        Idle,
        SearchForItem,
        SearchPlayer,
        Attack,
        Evade
    }

    [SerializeField] protected AttackType attackType;
    public enum AttackType
    {
        LongRange,
        CloseRange
    }
    public AttackType GetAttackType() => attackType;

    [SerializeField] protected StratagieType stratagieType;
    public enum StratagieType
    {
        Aggresive,
        Devensive,
        Strategic
    }
    public StratagieType GetStratagie() => stratagieType;

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

    // Effect stuff
    protected List<BaseEffect> enemyEffects = new List<BaseEffect>();
    public void AddEffect(BaseEffect effect)
    {
        enemyEffects.Add(effect);
    }

    public virtual void Start()
    {
        rb.linearDamping = airResistance;

        ApplyModifiers();

        //Idle idle = new Idle();
        //SearchForItem item = new SearchForItem();
        //SearchPlayer player = new SearchPlayer();
        //Attack attack = new Attack();
        //Evade evade = new Evade();

        //baseStates.Add(idle);
        //baseStates.Add(item);
        //baseStates.Add(player);
        //baseStates.Add(attack);
        //baseStates.Add(evade);

        SwitchState(States.Idle);
    }

    public virtual void Update()
    {

        if (currentState != null)
        {
            currentState.OnUpdate();
        }

        if (lastDamageTime > 0)
        {
            lastDamageTime -= Time.deltaTime;
        }

        if (itemHeld != null)
        {
            itemHeld.transform.position = itemHolder.transform.position;
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

    public void SetDestination(GameObject newDestination, bool isAttackingTarget)
    {
        if (isAttackingTarget)
        {
            targetEnemy = newDestination;
        }
        agent.destination = newDestination.transform.position;
    }

    public void SwitchState(States state)
    {
        currentState = states[(int)state];
        currentState.StartState(this);
    }

    public void TakeDmg(float dmg)
    {
        if (lastDamageTime <= 0)
        {
            health -= dmg;
            lastDamageTime = 0.5f;
        }
    }
}
