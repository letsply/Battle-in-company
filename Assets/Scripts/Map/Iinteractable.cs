using UnityEngine;
using UnityEngine.InputSystem;

public interface Iinteractable
{
    public bool isInteractable { get; set; }
    public void InteractWith(InputAction.CallbackContext context);
    public void OnInteraction();
}
