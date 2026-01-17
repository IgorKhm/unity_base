using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController2D : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 8f;
    public float acceleration = 40f;
    public float deceleration = 60f;

    [Header("Jump")]
    public float jumpVelocity = 12f;
    public int maxJumps = 2;

    [Header("Ground Check")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundMask;

    // These will be set by PlayerInput -> Unity Events
    private Vector2 moveInput;
    private bool jumpPressedThisFrame;

    private Rigidbody2D rb;
    private int jumpsRemaining;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        jumpsRemaining = maxJumps;
    }
    
    private float stunnedUntil;

    public void Stun(float seconds)
    {
        stunnedUntil = Mathf.Max(stunnedUntil, Time.time + seconds);
    }

    void Update()
    {
        bool grounded = IsGrounded();
        if (grounded && rb.linearVelocity.y <= 0.05f)
            jumpsRemaining = maxJumps;

        if (jumpPressedThisFrame && jumpsRemaining > 0)
        {
            jumpsRemaining--;

            // Reset vertical velocity so double-jump feels consistent
            Vector2 v = rb.linearVelocity;
            v.y = 0f;
            rb.linearVelocity = v;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpVelocity);
        }

        // Consume the one-frame jump flag
        jumpPressedThisFrame = false;
    }

    void FixedUpdate()
    {
        if (Time.time < stunnedUntil) return;

        float inputX = moveInput.x;
        float targetSpeed = inputX * maxSpeed;

        float speedDiff = targetSpeed - rb.linearVelocity.x;
        float accelRate = (Mathf.Abs(targetSpeed) > 0.01f) ? acceleration : deceleration;

        
        rb.AddForce(Vector2.right * (speedDiff * accelRate));
    }

    // PlayerInput (Invoke Unity Events) will call this for Move
    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();
    }

    // ...and this for Jump
    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
            jumpPressedThisFrame = true;
    }

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
    }
}
