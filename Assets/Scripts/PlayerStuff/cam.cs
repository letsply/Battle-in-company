using UnityEngine;
using UnityEngine.InputSystem;

public class cam : MonoBehaviour
{
    [SerializeField] private Transform m_player;
    public float Sensitivity;
    private float _rotationY;
    private float _rotationX;

    public void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    public void LateUpdate()
    {
        Look();
    }
    public void Look()
    {
        _rotationY = Mathf.Clamp(_rotationY, -85, 85);

        transform.rotation = Quaternion.Euler(_rotationY, _rotationX, 0);
        m_player.rotation = Quaternion.Euler(0, _rotationX, 0);

    }

    public void LookInput(InputAction.CallbackContext context)
    {
        _rotationX += context.ReadValue<Vector2>().x * Sensitivity * Time.deltaTime;
        _rotationY -= context.ReadValue<Vector2>().y * Sensitivity * Time.deltaTime;
    }
}
