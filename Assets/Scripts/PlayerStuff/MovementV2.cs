using CustomInspector;
using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class MovementV2 : MonoBehaviour
{
    private Rigidbody _rb;
    private float _horizonal;
    private float _vertical;
    private bool _jump;
    private Vector3 _moveDirection;

    public GameObject StaminaContainer;
    public GameObject Bottom;
    public Camera Cam;
    public Image StaminaBar;
    //public TMP_Text SpeedText;
    public float FOV;

    #region WalkStuff
    [Header("Walking")]
    public bool ShowWalkVars = true;

    private float _maxStamina;
    private bool _sprinting;
    private float _fOVMultiplyer;

    [ShowIf(nameof(ShowWalkVars))][SerializeField] private float m_speed;
    [ShowIf(nameof(ShowWalkVars))][SerializeField] private float m_sprint;
    [ShowIf(nameof(ShowWalkVars))][SerializeField] private float m_stamina;
    [ShowIf(nameof(ShowWalkVars))][SerializeField] private float m_staminaRegain;
    [ShowIf(nameof(ShowWalkVars))][SerializeField] private bool m_infinitStamina;
    [ShowIf(nameof(ShowWalkVars))] public LayerMask FloorMask;
    #endregion

    #region Jump
    [Header("Jump")]
    public bool ShowJumpVars = true;

    [ShowIf(nameof(ShowJumpVars))][SerializeField] private float m_jumpForce;
    [ShowIf(nameof(ShowJumpVars))][SerializeField] private float m_jumps;
    [ShowIf(nameof(ShowJumpVars))][SerializeField] private float m_airFriction;

    private float _maxJumps;
    private bool _OnFloor;

    #endregion

    #region Wall Actions
    [Header("WallStuff")]
    public bool ShowWallVars = true;

    [ShowIf(nameof(ShowWallVars))][SerializeField][Range(0, 1)] private float m_wallRunningSack;
    [ShowIf(nameof(ShowWallVars))][SerializeField] private float m_wallJumpForce;
    [ShowIf(nameof(ShowWallVars))][SerializeField] private float m_wallRunningSpeed;
    [ShowIf(nameof(ShowWallVars))][SerializeField] private bool m_enableWallRunning;
    [ShowIf(nameof(ShowWallVars))] public float _wallRunningRange;
    [ShowIf(nameof(ShowWallVars))] public LayerMask WallMask;
    private bool m_wallRunning;
    #endregion

    void Start()
    {
        _rb = GetComponent<Rigidbody>();

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        StaminaContainer.SetActive(false);
        _maxJumps = m_jumps;
        _maxStamina = m_stamina;
    }

    void FixedUpdate()
    {
        // Player Speed as text
        //float speed = Mathf.Abs(_rb.linearVelocity.x) + Mathf.Abs(_rb.linearVelocity.z);
        //SpeedText.text = Math.Round(speed,3).ToString();
        // StaminaBar
        StaminaBar.fillAmount = m_stamina / _maxStamina;

        Move();

        if (m_enableWallRunning == true)
        { WallRunn(); }

    }

    void Move()
    {
        // Desired Running Speed Determined by set speed and direction or if sprint button pressed direction,speed and sprint speed
        _moveDirection = _horizonal * transform.right + _vertical * transform.forward;
        _moveDirection.Normalize();

        #region Sprinting
        // The speed that the player trys to accel to
        float _targetSpeed = (_sprinting && m_stamina > 0 && _OnFloor || _sprinting && m_infinitStamina && _OnFloor) ? m_sprint : m_speed;

        // if sprint show stamina bar and remove stamina if not then fill up and dont show it when its full
        if (_sprinting && m_stamina > 0 && m_infinitStamina == false && _OnFloor)
        {
            m_stamina -= 1 * Time.deltaTime;
            StaminaContainer.SetActive(true);
        }
        else if (m_stamina <= _maxStamina && _sprinting == false && m_infinitStamina == false && _OnFloor)
        {
            m_stamina += m_staminaRegain * Time.deltaTime;
            if (m_stamina >= _maxStamina)
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

        if (_OnFloor)
        {
            if (Physics.Raycast(Bottom.transform.position, transform.TransformDirection(Vector3.forward), out AHit, 0.3f, FloorMask))
            {
                Debug.DrawRay(Bottom.transform.position + transform.TransformDirection(Vector3.forward) * 0.3f, transform.TransformDirection(Vector3.up), Color.red);
                if (Physics.Raycast(Bottom.transform.position + transform.TransformDirection(Vector3.forward) * 0.3f, transform.TransformDirection(Vector3.up), out OHit, FloorMask))
                {
                    float adjacent = 0.3f - AHit.distance;
                    float oposite = OHit.distance;
                    float ratio = oposite / adjacent;
                    if (ratio < 1)
                    {
                        _moveDirection.y = ratio;
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
                        _moveDirection.y = ratio;
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
                        _moveDirection.y = ratio;
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
                        _moveDirection.y = ratio;
                    }
                }
            }


        }
        else
        {
            _moveDirection.y = 0;
        }

        #endregion

        if (_OnFloor)
        {
            _rb.AddForce(_moveDirection * _targetSpeed, ForceMode.Force);
            Debug.DrawRay(Bottom.transform.position, _moveDirection, Color.blue);
        }
        else
        {
            _rb.AddForce(_moveDirection * _targetSpeed * m_airFriction, ForceMode.Force);
        }

        if (_rb.linearVelocity.x >= _targetSpeed && _rb.linearVelocity.z >= _targetSpeed)
        {
            _rb.AddForce(-_moveDirection * _targetSpeed, ForceMode.Force);
        }

        #endregion

        // Cam.fieldOfView = FOV + 25 * (Mathf.Abs(_rb.linearVelocity.x) + Mathf.Abs(_rb.linearVelocity.z)) / _targetSpeed;

        if (_jump && _OnFloor && m_jumps == _maxJumps)
        {
            _moveDirection.z = 0;
            _rb.AddForce(Vector3.up * m_jumpForce, ForceMode.VelocityChange);
            m_jumps--;
        }
    }
    void WallRunn()
    {
        RaycastHit leftSideRaycast;
        RaycastHit rightSideRaycast;
        RaycastHit FrontSideRaycast;
        RaycastHit BackSideRaycast;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out leftSideRaycast, _wallRunningRange, WallMask) && _OnFloor == false && MathF.Abs(_rb.linearVelocity.x) > 0 || Physics.Raycast(transform.position, transform.TransformDirection(Vector3.left), out rightSideRaycast, _wallRunningRange, WallMask) && _OnFloor == false && MathF.Abs(_rb.linearVelocity.x) > 0)
        {
            m_jumps = _maxJumps;
            if (_rb.linearVelocity.y <= 0 && _jump == false)
            {
                _rb.AddForce(Vector3.up * Mathf.Abs(_rb.linearVelocity.y * m_wallRunningSack), ForceMode.VelocityChange);
                _rb.AddForce(_moveDirection * Mathf.Abs(m_wallRunningSpeed), ForceMode.Force);
                m_wallRunning = true;
            }
            else if (_jump && m_jumps == _maxJumps)
            {
                _rb.AddForce(Vector3.up * m_wallJumpForce * 2, ForceMode.VelocityChange);

                if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.right), out leftSideRaycast, _wallRunningRange, WallMask))
                {
                    _rb.AddForce(transform.right * -1 * m_wallJumpForce, ForceMode.VelocityChange);
                }
                else
                {
                    _rb.AddForce(transform.right * m_wallJumpForce, ForceMode.VelocityChange);
                }

                m_jumps--;
            }
        }
        else { m_wallRunning = false; }

        #region WallJumping

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out FrontSideRaycast, _wallRunningRange, WallMask) && _OnFloor == false && _jump)
        {
            m_jumps = _maxJumps;
            _rb.AddForce(Vector3.up * m_wallJumpForce * 2, ForceMode.VelocityChange);
            _rb.AddForce(transform.forward * -1 * m_wallJumpForce, ForceMode.VelocityChange);
            m_jumps--;
        }
        else if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.back), out BackSideRaycast, _wallRunningRange, WallMask) && _OnFloor == false && _jump)
        {
            m_jumps = _maxJumps;
            _rb.AddForce(Vector3.up * m_wallJumpForce * 2, ForceMode.VelocityChange);
            _rb.AddForce(transform.forward * m_wallJumpForce, ForceMode.VelocityChange);
            m_jumps--;
        }

        #endregion

        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.left), Color.white, _wallRunningRange);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.right), Color.white, _wallRunningRange);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.forward), Color.white, _wallRunningRange);
        //Debug.DrawRay(transform.position, transform.TransformDirection(Vector3.back), Color.white, _wallRunningRange);
    }

    #region Input
    public void MoveInput(InputAction.CallbackContext context)
    {
        _horizonal = context.ReadValue<Vector2>().x;
        _vertical = context.ReadValue<Vector2>().y;
    }

    public void Jump(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _jump = true;
        }
        if (context.performed && m_jumps >= 1 && _OnFloor == false && m_wallRunning == false)
        {
            float doublejumpF = (_rb.linearVelocity.y < 0) ? m_jumpForce + Mathf.Abs(_rb.linearVelocity.y) : m_jumpForce;
            _rb.AddForce(Vector3.up * doublejumpF, ForceMode.VelocityChange);
            m_jumps--;
        }
        if (context.canceled)
        {
            _jump = false;
        }
    }

    public void sprint(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            _sprinting = true;
        }
        if (context.canceled)
        {
            _sprinting = false;
        }
    }

    #endregion

    #region Collision
    private void OnTriggerStay(Collider collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            _OnFloor = true;
            m_jumps = _maxJumps;
            _rb.linearDamping = 1;
        }
    }
    private void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.tag == "Floor")
        {
            _OnFloor = false;
            _rb.linearDamping = 0.5f;
        }
    }

    #endregion
}
