using UnityEngine;

public class Item : MonoBehaviour
{
    private bool itemIsHeld;
    private bool itemCanHurt;
    private bool playerIsPunching;

    public bool PlayerIsPunching(bool isPunching) => playerIsPunching = isPunching;

    // Get rb and velocity 
    private Rigidbody rb { get => GetComponent<Rigidbody>(); }
    private float velocity { get => Mathf.Abs(rb.linearVelocity.x) + Mathf.Abs(rb.linearVelocity.z) + Mathf.Abs(rb.linearVelocity.y); }

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

        }
    }
}
