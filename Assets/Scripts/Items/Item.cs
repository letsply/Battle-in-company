using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Item : MonoBehaviour, IModifieable
{
    private Player playerHolding;
    public void PlayerHolding(Player player) { playerHolding = player; }

    private EnemyBase enemyHolding;
    public void EnemyHolding(EnemyBase enemy) { enemyHolding = enemy; }

    #region standerdValues
    [SerializeField] private ItemData itemData;

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

    // Get rb and velocity 
    private Rigidbody rb { get => GetComponent<Rigidbody>(); }
    private float velocity { get => Mathf.Abs(rb.linearVelocity.x) + Mathf.Abs(rb.linearVelocity.z) + Mathf.Abs(rb.linearVelocity.y); }

    private void Start()
    {
        ApplyModifiers();
        hardness = itemData.Hardness();
        weight = itemData.Weight();
        handiness = itemData.Handiness();
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

    void FixedUpdate()
    {
        // Check Velocity if itemIsHeld or if playerIsUsing it to determin if the item can hurt
        if (velocity >= 1 && itemIsHeld == false || velocity >= 1 && isPunching)
        { itemCanHurt = true; }
        else
        { itemCanHurt = false; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Asking what is hit and if action has to be done
        if (itemCanHurt && collision.transform.tag == "Player")
        {
            // ask if item is punched with or if its thrown
            if (isPunching)
            {
                float damage = 0;
                // calc damage and the crit chance
                if (enemyHolding == null)
                { damage = 1 / 2 * weight * Mathf.Pow(playerHolding.GetStrenght(), 2) * hardness; }
                else if ( playerHolding == null)
                { damage = 1 / 2 * weight * Mathf.Pow(enemyHolding.GetStrenght(), 2) * hardness; }

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
                    { damage *= playerHolding.GetCriticalDamageMultiplier(); }
                    else if (playerHolding == null)
                    { damage *= enemyHolding.GetCriticalDamageMultiplier(); }
                }
                // let the player take dmg
                collision.transform.GetComponent<Player>().TakeDmg(damage);
            }
            else // if thrown do 
            {
                float damage = 1 / 2 * weight * Mathf.Pow(velocity, 2) * hardness;

                foreach (var mod in itemModifiers)
                {
                    mod.OnItemHit(ref damage);
                }

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
            // ask if item is punched with or if its thrown
            if (isPunching)
            {
                // calc damage and the crit chance
                float damage = 0;
                if (enemyHolding == null)
                { damage = 1 / 2 * weight * Mathf.Pow(playerHolding.GetStrenght(), 2) * hardness; }
                else if (playerHolding == null)
                { damage = 1 / 2 * weight * Mathf.Pow(enemyHolding.GetStrenght(), 2) * hardness; }

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
                    { damage *= playerHolding.GetCriticalDamageMultiplier(); }
                    else if (playerHolding == null)
                    { damage *= enemyHolding.GetCriticalDamageMultiplier(); }
                }

                // let the Enemy take dmg
                collision.transform.GetComponent<EnemyBase>().TakeDmg(damage);
            }
            else // if thrown do 
            {
                float damage = 1 / 2 * weight * Mathf.Pow(velocity, 2) * hardness;

                foreach (var mod in itemModifiers)
                {
                    mod.OnItemHit(ref damage);
                }

                collision.transform.GetComponent<Player>().TakeDmg(damage);
           

                // in the end apply effect to Enemy
                foreach (var mod in itemModifiers)
                {
                    mod.GiveEffect(null, collision.transform.GetComponent<EnemyBase>());
                }
            }
        }
    }
}
