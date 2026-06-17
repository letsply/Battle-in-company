using System.Reflection;
using UnityEngine;
using UnityEngine.AI;
using System.Collections.Generic;
using System.Collections;

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
    itemHolderAnim,
    perfectWeight
}


public class EnemyBase2 : MonoBehaviour
{
    #region Commponents

    [Header("Commponents & ChildObject stuff")]
    protected Rigidbody rb => gameObject.GetComponent<Rigidbody>();

    [SerializeField] protected GameObject itemHolder;
    public Animator ItemHolderAnim { get => itemHolder.GetComponent<Animator>(); }

    [SerializeField] protected GameObject itemHeld;
    [SerializeField] protected LayerMask itemLayer;
    public GameObject ItemHeld() => itemHeld;

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

    [SerializeField] protected float strength = 1000;
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

    // view Stuff
    protected EnemyView view;
    public EnemyView View { get => view; }
    public List<GameObject> TargetsInView() => View.TargetsInView();
    public bool TargetIsAttackable() => View.TargetIsAttackable();

    public bool HasItem()
    {
        if (itemHeld != null) { return true; } else { return false; }
    }

    public NavMeshAgent Agent { get => GetComponent<NavMeshAgent>(); }

    protected GameObject targetEnemy;
    public GameObject TargetEnemy() => targetEnemy;
    protected Vector3 targetPos;

    // State Stuff
    protected BaseState currentState;
    public BaseState CurrentState() => currentState;
    protected List<BaseState> states = new List<BaseState>();

    #region To Many Enums(states & Behavior)

    public enum States
    {
        Idle,
        SearchForItem,
        SearchPlayer,
        Attack,
    }

    [Header("Behavior")]
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

        //Give Itself to View
        view = new EnemyView();
        View.GetEnemyBase(this);

        // Initialize States
        Idle idle = new Idle();
        SearchItem item = new SearchItem();
        SearchPlayer player = new SearchPlayer();
        Attack attack = new Attack();

        states.Add(idle);
        states.Add(item);
        states.Add(player);
        states.Add(attack);

        SwitchState(States.Idle);
    }

    public virtual void Update()
    {

        if (currentState != null)
        {
            currentState.OnUpdate();
            View.OnUpdate();
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

    #region actions
    public void Take()
    {
        Collider[] hitColliders = Physics.OverlapBox(itemHolder.transform.position, new Vector3(grabbingRange, GetComponent<CapsuleCollider>().height, grabbingRange), Quaternion.identity, itemLayer);

        if (hitColliders.Length != 0)
        {
            // first collider in array gets used
            itemHeld = hitColliders[0].transform.gameObject;
            itemHeld.transform.parent = itemHolder.transform;
            itemHeld.transform.position = itemHolder.transform.position;
            itemHeld.transform.rotation = itemHolder.transform.rotation;

            itemHeld.GetComponent<Rigidbody>().useGravity = false;
            itemHeld.GetComponent<Rigidbody>().freezeRotation = true;
            itemHeld.GetComponent<Collider>().enabled = false;
                
            if (itemHeld.TryGetComponent<Item>(out Item item))
            {
                item.EnemyHolding(this);
            }
        }
    }

    public void Hit()
    {
        if (itemHeld.TryGetComponent<Item>(out Item item) && hitting == false)
        {
            StartCoroutine(HitAnimate(item));
        }
    }

    public IEnumerator HitAnimate(Item item)
    {
        ItemHolderAnim.SetTrigger("Punching");

        hitting = true;
        item.IsPunching(true);

        itemHeld.GetComponent<Collider>().enabled = true;
        itemHeld.GetComponent<Collider>().isTrigger = true;
        // Wait until the animation state is fully played (normalizedTime >= 1)
        yield return new WaitForSeconds(ItemHolderAnim.GetCurrentAnimatorStateInfo(0).length);

        // Animation has finished
        itemHeld.GetComponent<Collider>().enabled = false;
        itemHeld.GetComponent<Collider>().isTrigger = false;
        item.IsPunching(false);
        hitting = false;
    }

    public void Throw()
    {
        if (itemHeld.TryGetComponent<Item>(out Item item) && hitting == false)
        {
            StartCoroutine(ThrowAnimate(item));
        }
    }

    public IEnumerator ThrowAnimate(Item item)
    {
        ItemHolderAnim.SetTrigger("Throwing");

        // Wait until the animation state is fully played (normalizedTime >= 1)
        yield return new WaitForSeconds(ItemHolderAnim.GetCurrentAnimatorStateInfo(0).length);

        // Animation has finished
        itemHeld.GetComponent<Rigidbody>().useGravity = true;
        itemHeld.GetComponent<Rigidbody>().freezeRotation = false;
        itemHeld.GetComponent<Collider>().enabled = true;

        itemHolder.transform.DetachChildren();

        float a = strength / itemHeld.GetComponent<Rigidbody>().mass;
        float v = Mathf.Sqrt(2 * a * 0.5f);
        itemHeld.GetComponent<Rigidbody>().AddForce(v * transform.TransformDirection(Vector3.forward) + new Vector3(0, 0.5f), ForceMode.VelocityChange);
        itemHeld.GetComponent<Item>().EnemyHolding(null);

        itemHeld = null;
    }

    public void Use()
    {

    }

    #endregion

    public void SetDestination(GameObject newDestination, bool isAttackingTarget)
    {
        if (isAttackingTarget)
        {
            targetEnemy = newDestination;
        }
        Agent.destination = newDestination.transform.position;
    }

    public void SwitchState(States state)
    {
        currentState = states[(int)state];
        currentState.StartState(this);
    }
    public bool IsStateCurrent(States state)
    {
        if (currentState == states[(int)state])
        { return true; }
        else 
        { return false; }
    }

    public void TakeDmg(float dmg)
    {
        if (lastDamageTime <= 0)
        {
            health -= dmg;
            lastDamageTime = 0.5f;
            //Die
            if (health <= 0)
            {
                Destroy(gameObject);
            }
        }
    }

}
