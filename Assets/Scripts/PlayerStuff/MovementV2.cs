using CustomInspector;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovementV2 : MonoBehaviour
{
    private Player player { get => gameObject.GetComponent<Player>(); }

    private Rigidbody rb;
    private float horizonal;
    private float vertical;
    private bool jump;
    private Vector3 moveDirection;

    public GameObject StaminaContainer;
    public GameObject Bottom;
    public Camera Cam;
    public Image StaminaBar;
    //public TMP_Text SpeedText;
    public float FOV;

    #region WalkStuff
    [Header("Walking")]
    public bool ShowWalkVars = true;

    private float maxStamina;
    private bool sprinting;

    private float speed;
    private float sprint;
    private float stamina;
    private float staminaRegain;
    private bool infinitStamina;

    public LayerMask FloorMask;
    #endregion

    #region Jump
    [Header("Jump")]
    public bool ShowJumpVars = true;

    private float jumpForce;
    private float jumps;
    private float airFriction;

    private float maxJumps;
    private bool OnFloor;

    #endregion

    #region Wall Actions
    //[Header("WallStuff")]
    //public bool ShowWallVars = true;

    //[ShowIf(nameof(ShowWallVars))][SerializeField][Range(0, 1)] private float m_wallRunningSack;
    //[ShowIf(nameof(ShowWallVars))][SerializeField] private float m_wallJumpForce;
    //[ShowIf(nameof(ShowWallVars))][SerializeField] private float m_wallRunningSpeed;
    //[ShowIf(nameof(ShowWallVars))][SerializeField] private bool m_enableWallRunning;
    //[ShowIf(nameof(ShowWallVars))] public float _wallRunningRange;
    //[ShowIf(nameof(ShowWallVars))] public LayerMask WallMask;
    private bool wallRunning;
    #endregion

    void Start()
    {

        rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StaminaContainer.SetActive(false);
        maxJumps = jumps;
        maxStamina = stamina;
    }

    void FixedUpdate()
    {
        // Player Speed as text
        //float speed = Mathf.Abs(rb.linearVelocity.x) + Mathf.Abs(_rb.linearVelocity.z);
        //SpeedText.text = Math.Round(speed,3).ToString();
        
        // StaminaBar
        StaminaBar.fillAmount = stamina / maxStamina;

        Move();

        //if (enableWallRunning == true)
        //{ WallRunn(); }

    }

    void Move()
    {
        // Desired Running Speed Determined by set speed and direction or if sprint button pressed direction,speed and sprint speed
        moveDirection = horizonal * transform.right + vertical * transform.forward;
        moveDirection.Normalize();

        #region Sprinting
        // The speed that the player trys to accel to
        float targetSpeed = (sprinting && stamina > 0 && OnFloor || sprinting && infinitStamina && OnFloor) ? sprint : speed;

        // if sprint show stamina bar and remove stamina if not then fill up and dont show it when its full
        if (sprinting && stamina > 0 && infinitStamina == false && OnFloor)
        {
            stamina -= 1 * Time.deltaTime;
            StaminaContainer.SetActive(true);
        }
        else if (stamina <= maxStamina && sprinting == false && infinitStamina == false && OnFloor)
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
        Debug.DrawRay(Bottom.transform.position, transform.TransformDirection(Vector3.forward) * 0.3f, Color.green);

        if (OnFloor)
        {
            if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.forward), out AHit, 0.3f, FloorMask))
            {
                //Debug.DrawRay(Bottom.transform.position + transform.TransformDirection(Vector3.forward) * 0.3f, transform.TransformDirection(Vector3.up), Color.red);
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.forward) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, FloorMask))
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
            else if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.back), out AHit, 0.3f, FloorMask))
            {
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.back) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, FloorMask))
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
            else if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.left), out AHit, 0.3f, FloorMask))
            {
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.left) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, FloorMask))
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
            else if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.right), out AHit, 0.3f, FloorMask))
            {
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.right) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, FloorMask))
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

        if (OnFloor)
        {
            rb.AddForce(moveDirection * targetSpeed, ForceMode.Force);
            //Debug.DrawRay(Bottom.transform.position, moveDirection, Color.blue);
        }
        else
        {
            rb.AddForce(moveDirection * targetSpeed * airFriction, ForceMode.Force);
        }

        if (rb.linearVelocity.x >= targetSpeed && rb.linearVelocity.z >= targetSpeed)
        {
            rb.AddForce(-moveDirection * targetSpeed, ForceMode.Force);
        }

        #endregion

        // Cam.fieldOfView = FOV + 25 * (Mathf.Abs(_rb.linearVelocity.x) + Mathf.Abs(_rb.linearVelocity.z)) / _targetSpeed;

        if (jump && OnFloor && jumps == maxJumps)
        {
            moveDirection.z = 0;
            rb.AddForce(Vector3.up * jumpForce, ForceMode.VelocityChange);
            jumps--;
        }
    }

    //void WallRunn()
    //{
    //    RaycastHit leftSideRaycast;
    //    RaycastHit rightSideRaycast;
    //    RaycastHit FrontSideRaycast;
    //    RaycastHit BackSideRaycast;

    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out leftSideRaycast, _wallRunningRange, WallMask) && _OnFloor == false && MathF.Abs(_rb.linearVelocity.x) > 0 || Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out rightSideRaycast, _wallRunningRange, WallMask) && _OnFloor == false && MathF.Abs(_rb.linearVelocity.x) > 0)
    //    {
    //        m_jumps = _maxJumps;
    //        if (_rb.linearVelocity.y <= 0 && _jump == false)
    //        {
    //            _rb.AddForce(Vector3.up * Mathf.Abs(_rb.linearVelocity.y * m_wallRunningSack), ForceMode.VelocityChange);
    //            _rb.AddForce(_moveDirection * Mathf.Abs(m_wallRunningSpeed), ForceMode.Force);
    //            m_wallRunning = true;
    //        }
    //        else if (_jump && m_jumps == _maxJumps)
    //        {
    //            _rb.AddForce(Vector3.up * m_wallJumpForce * 2, ForceMode.VelocityChange);

    //            if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out leftSideRaycast, _wallRunningRange, WallMask))
    //            {
    //                _rb.AddForce(transform.right * -1 * m_wallJumpForce, ForceMode.VelocityChange);
    //            }
    //            else
    //            {
    //                _rb.AddForce(transform.right * m_wallJumpForce, ForceMode.VelocityChange);
    //            }

    //            m_jumps--;
    //        }
    //    }
    //    else { m_wallRunning = false; }

    //    #region WallJumping

    //    if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out FrontSideRaycast, _wallRunningRange, WallMask) && _OnFloor == false && _jump)
    //    {
    //        m_jumps = _maxJumps;
    //        _rb.AddForce(Vector3.up * m_wallJumpForce * 2, ForceMode.VelocityChange);
    //        _rb.AddForce(transform.forward * -1 * m_wallJumpForce, ForceMode.VelocityChange);
    //        m_jumps--;
    //    }
    //    else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out BackSideRaycast, _wallRunningRange, WallMask) && _OnFloor == false && _jump)
    //    {
    //        m_jumps = _maxJumps;
    //        _rb.AddForce(Vector3.up * m_wallJumpForce * 2, ForceMode.VelocityChange);
    //        _rb.AddForce(transform.forward * m_wallJumpForce, ForceMode.VelocityChange);
    //        m_jumps--;
    //    }

    //    #endregion

    //    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left), Color.white, _wallRunningRange);
    //    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right), Color.white, _wallRunningRange);
    //    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.white, _wallRunningRange);
    //    //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back), Color.white, _wallRunningRange);
    //}

    #region Input
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
        if (context.performed && jumps >= 1 && OnFloor == false && wallRunning == false)
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

    #region Collision
    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            OnFloor = true;
            jumps = maxJumps;
            rb.linearDamping = 1;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            OnFloor = false;
            rb.linearDamping = 0.5f;
        }
    }

    #endregion
}
