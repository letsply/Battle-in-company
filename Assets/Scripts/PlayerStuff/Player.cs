using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class Player : MonoBehaviour, IModifieable
{
    [SerializeField] private float strenght;
    [SerializeField] private float health;
    [SerializeField] private float speed;

    public float Strenght(float newStrenght) => strenght = newStrenght;
    public float Health(float newHealth) => health = newHealth;
    public float Speed(float newSpeed) => speed = newSpeed;

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
    public void HitInput(InputAction.CallbackContext context)
    {

    }

    public void ThrowInput(InputAction.CallbackContext context)
    {

    }
    #endregion
   
}

