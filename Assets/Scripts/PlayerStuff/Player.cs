using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;


public class Player : MonoBehaviour, IModifieable
{
    [SerializeField] private float strenght;
    [SerializeField] private float health;
    [SerializeField] private float speed;

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

