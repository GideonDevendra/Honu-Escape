using UnityEngine;
#if ENABLE_INPUT_SYSTEM || UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

[RequireComponent(typeof(Rigidbody2D))]
public class Movement : MonoBehaviour
{
    [Header("Movement Settings")]
    [Tooltip("Maximum movement speed in units/second.")]
    public float maxSpeed = 5f;

    [Tooltip("Acceleration rate (units/second^2) while input is held.")]
    public float acceleration = 40f;

    [Tooltip("Deceleration rate (units/second^2) when input is released.")]
    public float deceleration = 60f;

    // Internal state
    private Rigidbody2D _rb;
    private Vector2 _velocity;

    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        Vector2 input = Vector2.zero;

#if ENABLE_INPUT_SYSTEM || UNITY_INPUT_SYSTEM
        // Read keyboard (WASD/Arrows)
        if (Keyboard.current != null)
        {
            float x = 0f;
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) x -= 1f;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) x += 1f;

            float y = 0f;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) y -= 1f;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) y += 1f;

            input += new Vector2(x, y);
        }

        // Read gamepad left stick if present
        if (Gamepad.current != null)
        {
            input += Gamepad.current.leftStick.ReadValue();
        }
#else
        // Legacy input system fallback (only used if new Input System isn't enabled)
        input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
#endif

        // If combined input > 1 (e.g. keyboard + stick or diagonal), normalize to cap diagonal speed
        if (input.sqrMagnitude > 1f)
            input.Normalize();

        // Desired target velocity based on input
        Vector2 targetVelocity = input * maxSpeed;

        // Choose accel or decel depending on whether there is input
        float rate = (input.sqrMagnitude > 0f) ? acceleration : deceleration;

        // Smoothly move current velocity toward target velocity
        _velocity = Vector2.MoveTowards(_velocity, targetVelocity, rate * Time.fixedDeltaTime);

        // Apply velocity to Rigidbody2D
        _rb.linearVelocity = _velocity;
    }
}
