using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour, IModifieable
{
    #region Components
    private Rigidbody rb { get => gameObject.GetComponent<Rigidbody>(); }
    [SerializeField] private GameObject Bottom;
    [SerializeField] private GameObject itemHolder;
    [SerializeField] private GameObject Cam;
    private GameObject itemHolding;

    #endregion

    #region Values

    [Header("BaseValues")]
    [SerializeField] private float strenght;
    [SerializeField] private float health;
    [SerializeField] private float criticalDamageMultiplier;

    #region Movement(Walking)

    [Header("MovementWalkValues")]
    [SerializeField] private float speed;
    [SerializeField] private float sprintingSpeed;
    [SerializeField] private float maxStamina;
    [SerializeField] private float staminaRegain;
    [SerializeField] private bool infinitStamina;
    private float stamina;

    #endregion

    #region Movement(Jump)

    [Header("MovementWalkValues")]
    [SerializeField] private float jumpForce;
    [SerializeField] private int jumps; // The amount off Jumps the player has
    [SerializeField] private float airResistance;

    private int maxJumps;
    #endregion

    public enum Values
    {
        strength,
        health,
        criticalDamageMultiplier,
        speed,
        sprintingSpeed,
        stamina,
        staminaRegain,
        infinitStamina,
        jumpForce,
        jumps,
        airResistance,
    }
    private string[] valueNames = new string[]
    {
        "strength",
        "health",
        "criticalDamageMultiplier",
        "speed",
        "sprintingSpeed",
        "stamina",
        "staminaRegain",
        "infinitStamina",
        "jumpForce",
        "jumps",
        "airResistance",
    };

    public T GetValue<T>(Values valueName)
    {
        string name = valueNames[(int)valueName];
        var field = GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        return (T)field.GetValue(this);
    }

    public void SetValue<T>(Values valueName, T value)
    {
        var field = GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(this,value);
    }

    #endregion

    #region ModifierStuff
    private List<BaseEffect> playerEffects = new List<BaseEffect>();
    private List<PlayerBaseModifier> playerModifiers = new List<PlayerBaseModifier>();
    public void ApplyModifiers()
    {
        if (playerModifiers.Count > 0)
        {
            foreach (var mod in playerModifiers) {
                mod.Modify();
                mod.FindPlayer(this);
            }
        }
    }
    public void AddEffect(BaseEffect effect)
    {
        playerEffects.Add(effect);
    }

    #endregion

    #region inputStuff

    [SerializeField] private float itemGrabingRange = 1;
    private float horizonal;
    private float vertical;
    private Vector3 moveDirection;

    private bool jump;
    private bool sprinting;
    private bool onFloor;

    [SerializeField]private LayerMask floorMask;
    [SerializeField] private LayerMask itemMask;
    #endregion

    //UI
    public GameObject StaminaContainer;
    public Image StaminaBar;

    public void Start()
    {
        stamina = maxStamina;

        ApplyModifiers();
    }

    public void Update()
    {
        // Update HeldItemPos
        if (itemHolding != null)
        { itemHolding.transform.position = itemHolder.transform.position; }
    }

    public void FixedUpdate()
    {
        Move();

        // StaminaBar
        StaminaBar.fillAmount = stamina / maxStamina;

        for (int i = 0; i < playerEffects.Count; i++)
        {
            playerEffects[i].Update();
            if (playerEffects[i].GetDuration() <= 0)
            {
                playerEffects.RemoveAt(i);
            }
        }
    }

    public void TakeDmg(float dmg)
    {
         health -= dmg;
    }

    #region Movement
    void Move()
    {
        // Desired Running Speed Determined by set speed and direction or if sprint button pressed direction,speed and sprint speed
        moveDirection = horizonal * transform.right + vertical * transform.forward;
        moveDirection.Normalize();

        #region Sprinting
        // The speed that the player trys to accel to
        float targetSpeed = (sprinting && stamina > 0 && onFloor || sprinting && infinitStamina && onFloor) ? sprintingSpeed : speed;

        // if sprint show stamina bar and remove stamina if not then fill up and dont show it when its full
        if (sprinting && stamina > 0 && infinitStamina == false && onFloor)
        {
            stamina -= 1 * Time.deltaTime;
            StaminaContainer.SetActive(true);
        }
        else if (stamina <= maxStamina && sprinting == false && infinitStamina == false && onFloor)
        {
            stamina += staminaRegain * Time.deltaTime;
            if (stamina >= maxStamina)
            {
                StaminaContainer.SetActive(false);
            }
        }

        #endregion

        #region Force&Resistance

        #region Slope
        RaycastHit AHit;
        RaycastHit OHit;
        // Debug.DrawRay(Bottom.transform.position, transform.TransformDirection(Vector3.forward) * 0.3f, Color.green);

        if (onFloor)
        {
            if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.forward), out AHit, 0.3f, floorMask))
            {
                //Debug.DrawRay(Bottom.transform.position + transform.TransformDirection(Vector3.forward) * 0.3f, transform.TransformDirection(Vector3.up), Color.red);
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.forward) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, floorMask))
                {
                    float adjacent = 0.3f - AHit.distance;
                    float oposite = OHit.distance;
                    float ratio = oposite / adjacent;
                    if (ratio < 1)
                    {
                        moveDirection.y = ratio;
                    }
                }
            }
            else if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.back), out AHit, 0.3f, floorMask))
            {
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.back) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, floorMask))
                {
                    float adjacent = 0.3f - AHit.distance;
                    float oposite = OHit.distance;
                    float ratio = oposite / adjacent;
                    if (ratio < 1)
                    {
                        moveDirection.y = ratio;
                    }
                }
            }
            else if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.left), out AHit, 0.3f, floorMask))
            {
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.left) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, floorMask))
                {
                    float adjacent = 0.3f - AHit.distance;
                    float oposite = OHit.distance;
                    float ratio = oposite / adjacent;
                    if (ratio < 1)
                    {
                        moveDirection.y = ratio;
                    }
                }
            }
            else if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.right), out AHit, 0.3f, floorMask))
            {
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.right) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, floorMask))
                {
                    float adjacent = 0.3f - AHit.distance;
                    float oposite = OHit.distance;
                    float ratio = oposite / adjacent;
                    if (ratio < 1)
                    {
                        moveDirection.y = ratio;
                    }
                }
            }


        }
        else
        {
            moveDirection.y = 0;
        }

        #endregion

        if (onFloor)
        {
            rb.AddForce(moveDirection * targetSpeed, ForceMode.Force);
            //Debug.DrawRay(Bottom.transform.position, moveDirection, Color.blue);
        }
        else
        {
            rb.AddForce(moveDirection * targetSpeed * airResistance, ForceMode.Force);
        }

        if (rb.linearVelocity.x >= targetSpeed && rb.linearVelocity.z >= targetSpeed)
        {
            rb.AddForce(-moveDirection * targetSpeed, ForceMode.Force);
        }

        #endregion

        // Cam.fieldOfView = FOV + 25 * (Mathf.Abs(_rb.linearVelocity.x) + Mathf.Abs(_rb.linearVelocity.z)) / _targetSpeed;

        if (jump && onFloor && jumps == maxJumps)
        {
            moveDirection.z = 0;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            jumps--;
        }
    }
    #endregion

    #region Actions
    private void Hit()
    {

    }

    private void Throw()
    {

    }

    private void Use()
    {

    }
    #endregion

    #region Input

    #region MovementInput
    public void MoveInput(InputAction.CallbackContext context)
    {
        horizonal = context.ReadValue<Vector2>().x;
        vertical = context.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            jump = true;
        }
        if (context.performed && jumps >= 1 && onFloor == false)
        {
            float doublejumpF = (rb.linearVelocity.y < 0) ? jumpForce + Mathf.Abs(rb.linearVelocity.y) : jumpForce;
            rb.AddForce(Vector3.up * doublejumpF, ForceMode.VelocityChange);
            jumps--;
        }
        if (context.canceled)
        {
            jump = false;
        }
    }

    public void Sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            sprinting = true;
        }
        if (context.canceled)
        {
            sprinting = false;
        }
    }
    #endregion

    #region ItemStuff
    public void TakeItem(InputAction.CallbackContext context)
    {
        RaycastHit hit;
        if (Physics.Raycast(Cam.transform.position, Cam.transform.forward, out hit, itemGrabingRange, itemMask) && context.performed && itemHolding == null)
        {
            itemHolding = hit.transform.gameObject;
            itemHolding.transform.parent = itemHolder.transform;
            itemHolding.transform.position = itemHolder.transform.position;
            itemHolding.transform.rotation = itemHolder.transform.rotation;

            itemHolding.GetComponent<Rigidbody>().useGravity = false;
            itemHolding.GetComponent<Rigidbody>().freezeRotation = true;
            itemHolding.GetComponent<Collider>().enabled = false;
        }
    }

    public void DropItem(InputAction.CallbackContext context)
    {
        if (itemHolding != null && context.performed)
        {
            itemHolding.GetComponent<Rigidbody>().useGravity = true;
            itemHolding.GetComponent<Rigidbody>().freezeRotation = false;
            itemHolding.GetComponent<Collider>().enabled = true;
            itemHolding.GetComponent<Rigidbody>().AddForce(moveDirection * (rb.linearVelocity.x + rb.linearVelocity.y) , ForceMode.Impulse);

            itemHolding = null;
            itemHolder.transform.DetachChildren();
        }
    }

    public void HitInput(InputAction.CallbackContext context)
    {

    }

    public void ThrowInput(InputAction.CallbackContext context)
    {

    }

    public void UseInput(InputAction.CallbackContext context)
    {
        if(itemHolding != null)
        {
            if(itemHolding.TryGetComponent<Item>(out Item item))
            {
                item.UseItem();
            }
        }
    }
    #endregion

    #endregion

    #region Collision
    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            onFloor = true;
            jumps = maxJumps;
            rb.linearDamping = 1;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            onFloor = false;
            rb.linearDamping = 0.5f;
        }
    }

    #endregion

}

