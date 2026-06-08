using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Item : MonoBehaviour, IModifieable
{

    #region get&setPlayer/Enemy
    private Player playerHolding;
    public void PlayerHolding(Player player) { playerHolding = player; }

    private StateMachineControler enemyHolding;
    public void EnemyHolding(StateMachineControler enemy) { enemyHolding = enemy; }
    public bool ItemIsHeld() 
    {
        if(playerHolding == null && enemyHolding == null)
        { return false; }

        return true;
    }
    #endregion

    #region standerdValues
    [SerializeField] private ItemData itemData;
    public ItemData ItemsData() => itemData;

    // Values
    private float hardness;
    private float weight;
    private float handiness;

    // Item Action
    private bool itemIsHeld;
    private bool itemCanHurt;
    private bool isPunching;

    public bool IsPunching(bool isitPunching) => isPunching = isitPunching;
    #endregion


    private List<ItemBaseModifier> itemModifiers = new List<ItemBaseModifier>();
    public bool ItemIsUsable()
    {
        bool anyModierHasUse = false;
        foreach (var mod in itemModifiers)
        {
            if (mod.HasUse())
            {
                anyModierHasUse = true;
            }
        }
        return anyModierHasUse;
    }

    // Get rb and velocity 
    private Rigidbody rb { get => GetComponent<Rigidbody>(); }
    private float velocity { get => Mathf.Abs(rb.linearVelocity.x) + Mathf.Abs(rb.linearVelocity.z) + Mathf.Abs(rb.linearVelocity.y); }


    private void Start()
    {
        ApplyModifiers();
        hardness = itemData.Hardness();
        weight = itemData.Weight();
        handiness = itemData.Handiness();

        rb.mass = weight;
    }

    public void ApplyModifiers()
    {
        if (itemModifiers.Count > 0)
        {
            foreach (var mod in itemModifiers) {
                mod.FindItem(this);
                mod.Modify();
            }
        }
    }

    public void UseItem()
    {
        if (itemModifiers.Count > 0)
        {
            foreach (var mod in itemModifiers)
            {
                mod.OnItemUse();
            }
        }
    }

    void FixedUpdate()
    {
        // Check Velocity if itemIsHeld or if playerIsUsing it to determin if the item can hurt
        if (velocity >= 1 && itemIsHeld == false || isPunching)
        { itemCanHurt = true; }
        else
        { itemCanHurt = false; }
    }

    private void OnTriggerEnter(Collider collision)
    {
        // Asking what is hit and if action has to be done
        if (itemCanHurt && collision.transform.tag == "Player")
        {
            // ask if item is not hitting self
            if (collision.gameObject.GetComponent<Player>() != playerHolding)
            {
                float damage = 0;
                // calc damage and the crit chance
                // damage = 0.5 * weight * the velocity you would get with the weight of the object and 0.5m of assumed traveling time
                if (enemyHolding == null)
                {
                    float a = playerHolding.GetValue<float>(PlayerValues.strength) / weight;
                    float v = Mathf.Sqrt(2 * a * 0.5f);
                    damage = (1f / 2f) * weight * Mathf.Pow(v, 2) * hardness; 
                }
                else if ( playerHolding == null)
                {
                    float a = enemyHolding.GetValue<float>(EnemyValues.strength) / weight;
                    float v = Mathf.Sqrt(2 * a * 0.5f);
                    damage = (1f / 2f) * weight * Mathf.Pow(v, 2) * hardness;
                }

                float critchance = handiness;
                // modify damage and critchance
                foreach (var mod in itemModifiers)
                {
                    mod.OnItemPunchHit(ref damage, ref critchance);
                }
                // Randomize if crit should happen and multiply damage
                float crit = Random.Range(0f, 100f);
                if (crit <= critchance)
                {
                    if (enemyHolding == null)
                    { damage *= playerHolding.GetValue<float>(PlayerValues.criticalDamageMultiplier); }
                    else if (playerHolding == null)
                    { damage *= enemyHolding.GetValue<float>(EnemyValues.criticalDamageMultiplier); }
                }
                // let the Enemy take dmg and divide it by 10 to hold the numbers small
                damage /= 10;

                collision.transform.GetComponent<Player>().TakeDmg(damage);
            }

            // in the end apply effects to Player
            foreach (var mod in itemModifiers)
            {
                mod.GiveEffect(collision.transform.GetComponent<Player>(), null);
            }
        }
        else if (itemCanHurt && collision.transform.tag == "Enemy")
        {
            // ask if item is not hitting self
            if (collision.gameObject.GetComponent<StateMachineControler>() != enemyHolding)
            {
                // calc damage and the crit chance
                float damage = 0;

                if (enemyHolding == null)
                {
                    float a = playerHolding.GetValue<float>(PlayerValues.strength) / weight;
                    float v = Mathf.Sqrt(2 * a * 0.5f);
                    damage = (1f / 2f) * weight * Mathf.Pow(v, 2) * hardness;
                }
                else if (playerHolding == null)
                {
                    float a = enemyHolding.GetValue<float>(EnemyValues.strength) / weight;
                    float v = Mathf.Sqrt(2 * a * 0.5f);
                    damage = (1f / 2f) * weight * Mathf.Pow(v, 2) * hardness;
                }
                float critchance = handiness;

                // modify damage and critchance
                foreach (var mod in itemModifiers)
                {
                    mod.OnItemPunchHit(ref damage, ref critchance);
                }

                // Randomize if crit should happen and multiply damage
                float crit = Random.Range(0f, 100f);
                if (crit <= critchance)
                {
                    if (enemyHolding == null)
                    { damage *= playerHolding.GetValue<float>(PlayerValues.criticalDamageMultiplier); }
                    else if (playerHolding == null)
                    { damage *= enemyHolding.GetValue<float>(EnemyValues.criticalDamageMultiplier); }
                }

                // let the Enemy take dmg and divide it by 10 to hold the numbers small
                damage /= 10;

                collision.transform.GetComponent<StateMachineControler>().TakeDmg(damage);
            }
        }

        if (collision.gameObject.GetComponent<Rigidbody>() != null && itemIsHeld)
        {
            Vector3 direction = new Vector3();
            if(playerHolding == null)
            { direction = enemyHolding.GetComponent<Transform>().transform.TransformDirection(Vector3.forward); }
            else
            { direction = playerHolding.GetComponent<Transform>().transform.TransformDirection(Vector3.forward); }

            //making enemy bounce a bit
            direction += new Vector3(0,weight * 1.5f);

            float force = 0;

            if (enemyHolding == null)
            { force = playerHolding.GetValue<float>(PlayerValues.strength); }
            else if (playerHolding == null)
            { force = enemyHolding.GetValue<float>(EnemyValues.strength); }

            foreach (var mod in itemModifiers)
            {
                mod.OnKnockback(ref force);
            }

            collision.gameObject.GetComponent<Rigidbody>().AddForce(force * direction, ForceMode.Force);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (itemCanHurt && collision.transform.tag == "Player")
        {

            float damage = (1f / 2f) * weight * Mathf.Pow(velocity, 2) * hardness;

            foreach (var mod in itemModifiers)
            {
                mod.OnItemHit(ref damage);
            }

            // let the Enemy take dmg and divide it by 10 to hold the numbers small
            damage /= 10;

            collision.transform.GetComponent<Player>().TakeDmg(damage);

            // in the end apply effect to Enemy
            foreach (var mod in itemModifiers)
            {
                mod.GiveEffect(collision.transform.GetComponent<Player>(), null);
            }
        }
        else if (itemCanHurt && collision.transform.tag == "Enemy")
        {
            float damage = (1f / 2f) * weight * Mathf.Pow(velocity, 2) * hardness;

            foreach (var mod in itemModifiers)
            {
                mod.OnItemHit(ref damage);
            }

            // let the Enemy take dmg and divide it by 10 to hold the numbers small
            damage /= 10;

            collision.transform.GetComponent<StateMachineControler>().TakeDmg(damage);


            // in the end apply effect to Enemy
            foreach (var mod in itemModifiers)
            {
                mod.GiveEffect(null, collision.transform.GetComponent<StateMachineControler>());
            }
        }
    }
}
