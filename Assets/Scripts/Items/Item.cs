using JetBrains.Annotations;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Item : MonoBehaviour, IModifieable
{
    private Player playerHolding;
    public Player PlayerHolding(Player player) { return playerHolding = player; }

    #region standerdValues
    [SerializeField] private ItemData itemData;

    // Values
    private float hardness;
    private float weight;
    private float handiness;

    // Item Action
    private bool itemIsHeld;
    private bool itemCanHurt;
    private bool playerIsPunching;

    public bool PlayerIsPunching(bool isPunching) => playerIsPunching = isPunching;
    #endregion

    private List<float> ExtraModifierValues = new List<float>();
    private List<string> ExtraModifierDestinations = new List<string>();

    private List<ItemBaseModifier> itemModifiers = new List<ItemBaseModifier>();

    // Get rb and velocity 
    private Rigidbody rb { get => GetComponent<Rigidbody>(); }
    private float velocity { get => Mathf.Abs(rb.linearVelocity.x) + Mathf.Abs(rb.linearVelocity.z) + Mathf.Abs(rb.linearVelocity.y); }

    private void Start()
    {
        hardness = itemData.Hardness();
        weight = itemData.Weight();
        handiness = itemData.Handiness();
    }

    public void ApplyModifiers()
    {
        if (itemModifiers.Count > 0)
        {
            foreach (var mod in itemModifiers) { mod.Modify(); }
        }
    }

    void FixedUpdate()
    {
        // Check Velocity if itemIsHeld or if playerIsUsing it to determin if the item can hurt
        if (velocity >= 1 && itemIsHeld == false || velocity >= 1 && playerIsPunching)
        { itemCanHurt = true; }
        else
        { itemCanHurt = false; }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (itemCanHurt && collision.transform.tag == "Player")
        {
            if (playerIsPunching)
            {
                float damage = 1 / 2 * weight * Mathf.Pow(playerHolding.GetStrenght(),2) * hardness;
                float critchance = handiness;
               

                collision.transform.GetComponent<Player>().TakeDmg(damage);
            }
            else
            {

            }
        }
    }
}
