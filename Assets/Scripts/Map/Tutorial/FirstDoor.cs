using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class FirstDoor : MonoBehaviour,Iinteractable
{
    public bool isInteractable { get; set; }
    private Animator anim { get => GetComponent<Animator>(); }

    public void InteractWith(InputAction.CallbackContext context)
    {
        if (isInteractable)
        {
            OnInteraction();
        }
    }

    public void OnInteraction()
    {
        StartCoroutine(SwitchAnimation("Open"));
    }

    private IEnumerator SwitchAnimation(string animationName)
    {
        anim.ResetTrigger("Open");
        anim.ResetTrigger("Close");
        anim.SetTrigger(animationName);
        isInteractable = false;

        yield return new WaitForSeconds(anim.GetCurrentAnimatorStateInfo(0).length);
    }

    private void OnTriggerStay(Collider other)
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
            if (anim.GetCurrentAnimatorStateInfo(0).IsTag("Open"))
            {
                StartCoroutine(SwitchAnimation("Close")); 
            }
            isInteractable = false;
        }
    }
}
