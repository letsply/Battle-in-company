using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IModifieable
{
    #region Values
    [SerializeField] private float strenght;
    [SerializeField] private float health;
    [SerializeField] private float speed;

    public float GetStrenght() => strenght;
    public float GetHealth() => health;
    public float GetSpeed() => speed;

    public float SetStrenght(float newStrenght) => strenght = newStrenght;
    public float SetHealth(float newHealth) => health = newHealth;
    public float SetSpeed(float newSpeed) => speed = newSpeed;

    #endregion

    private List<float> ExtraModifierValues = new List<float>();
    private List<string> ExtraModifierDestinations = new List<string>();

    private List<PlayerBaseModifier> playerModifiers = new List<PlayerBaseModifier>();

    public void Start()
    {
        ApplyModifiers();
    }

    public void ApplyModifiers()
    {
        if (playerModifiers.Count > 0)
        {
            foreach (var mod in playerModifiers) { mod.Modify(); }
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

