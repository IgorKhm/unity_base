using UnityEngine;

public class EnemyWalker2D : MonoBehaviour
{
    public Transform target;

    [Header("Movement")]
    public float patrolSpeed = 2f;
    public float chaseSpeed = 3.5f;
    public float stopDistance = 1.0f;

    [Header("Grounding")]
    public Transform groundCheck;
    public float groundCheckRadius = 0.15f;
    public LayerMask groundMask;

    [Header("Obstacle checks")]
    public Transform wallCheck;
    public float wallCheckDistance = 0.2f;

    [Header("Ledge checks (optional)")]
    public Transform ledgeCheckRight;
    public Transform ledgeCheckLeft;
    public float ledgeCheckDistance = 0.4f;
    public bool avoidLedges = true;


    private Rigidbody2D rb;
    private bool stunned;
    private int patrolDir = 1;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb == null) rb = GetComponentInParent<Rigidbody2D>();    }

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
    }

    public void Stun(float seconds)
    {
        if (!gameObject.activeInHierarchy) return;
        CancelInvoke(nameof(EndStun));
        stunned = true;
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
        Invoke(nameof(EndStun), seconds);
    }

    void EndStun() => stunned = false;

    bool IsGrounded()
    {
        if (groundCheck == null) return false;
        return Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundMask) != null;
    }

    bool HitWall()
    {
        if (wallCheck == null) return false;
        var hit = Physics2D.Raycast(wallCheck.position, Vector2.right * patrolDir, wallCheckDistance, groundMask);
        return hit.collider != null;
    }

    bool HasGroundAhead(int dir)
    {
        if (!avoidLedges) return true;

        Transform check = (dir >= 0) ? ledgeCheckRight : ledgeCheckLeft;
        if (check == null) return true; // fail-open so it still moves

        var hit = Physics2D.Raycast(check.position, Vector2.down, ledgeCheckDistance, groundMask);
        return hit.collider != null;
    }

    float nextDebug;

    void FixedUpdate()
    {
        if (Time.time >= nextDebug)
        {
            nextDebug = Time.time + 2f;
            Debug.Log($"[WALKER] enabled={enabled} stunned={stunned} vel={rb.linearVelocity} patrolSpeed={patrolSpeed}");
        }

        if (stunned) return;

        var vision = GetComponent<EnemyVision2D>();
        bool canSee = (vision != null && vision.CanSeePlayer);
        Debug.Log($"[WALKER] canSee={canSee} target={(target?target.name:"null")} rb={(rb?rb.name:"null")}");
        if (canSee && target != null) Chase();
        else Patrol();
    }

    void Patrol()
    {
        // turn around on wall or edge
        if (HitWall() || !HasGroundAhead(patrolDir))
            patrolDir *= -1;

        float vx = patrolDir * patrolSpeed;
        rb.linearVelocity = new Vector2(patrolDir * patrolSpeed, rb.linearVelocity.y);
        // rb.linearVelocity = new Vector2(vx, rb.linearVelocity.y);
    }

    void Chase()
    {
        Vector2 toTarget = (Vector2)(target.position - transform.position);

        // Only move on X axis
        float absX = Mathf.Abs(toTarget.x);
        if (absX <= stopDistance)
        {
            Debug.Log("[CHASE] stopping: within stopDistance");
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        int dir = (toTarget.x >= 0f) ? 1 : -1;
        patrolDir = dir; // so wall checks face the right way

        // If we're avoiding ledges and there's no ground ahead, stop (prevents suicide walking off)
        if (!HasGroundAhead(dir))
        {
            Debug.Log("[CHASE] stopping: no ground ahead");
            rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);
            return;
        }

        rb.linearVelocity = new Vector2(dir * chaseSpeed, rb.linearVelocity.y);
        Debug.Log($"[CHASE] setting vx={dir * chaseSpeed}");
    }

    void OnDrawGizmosSelected()
    {
        if (groundCheck != null)
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);

        if (wallCheck != null)
            Gizmos.DrawLine(wallCheck.position, wallCheck.position + Vector3.right * patrolDir * wallCheckDistance);

        if (ledgeCheckLeft != null)
            Gizmos.DrawLine(ledgeCheckLeft.position, ledgeCheckLeft.position + Vector3.down * ledgeCheckDistance);
        if (ledgeCheckRight != null)
            Gizmos.DrawLine(ledgeCheckRight.position, ledgeCheckRight.position + Vector3.down * ledgeCheckDistance);
    }
}
