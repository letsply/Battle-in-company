using UnityEngine;
using UnityEngine.InputSystem;

public class FirstDoor : MonoBehaviour,Iinteractable
{
    public bool isInteractable { get; set; }
    public void InteractWith(InputAction.CallbackContext context)
    {
        if (isInteractable)
        {
            OnInteraction();
        }
    }

    public void OnInteraction()
    {

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            isInteractable = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            isInteractable = false;
        }
    }
}
