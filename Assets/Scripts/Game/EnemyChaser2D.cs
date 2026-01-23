using UnityEngine;

public class EnemyChaser2D : MonoBehaviour
{
    public Transform target;
    public float moveSpeed = 4f;
    public float stopDistance = 1f;

    private Rigidbody2D rb;
    private bool stunned;
    
    public float LastApproachXDir { get; private set; } = 1f;
    public float approachDeadZone = 0.15f; // don't update sign when nearly centered

    public float wanderRadius = 3f;
    public float wanderSpeed = 2f;
    public float wanderInterval = 2f;

    private Vector2 home;
    private Vector2 wanderTarget;
    private float nextWanderTime;
    
    public LayerMask obstacleMask;
    public float stuckSpeedThreshold = 0.1f;
    public float stuckTimeToRepath = 0.4f;

    private float stuckTimer;
    
    public float steerStrength = 8f;   // how quickly it turns/accelerates
    public float maxAccel = 20f;
    
    [Header("Wander feel")]
    public float arriveDistance = 0.6f;
    public float forwardBias = 0.75f;      // 0..1, higher = less random
    public float homeFollow = 0.2f;        // 0..1, how much "home" follows current position
    public float minTargetDistance = 1.2f; // prevents tiny hops
    
    public bool IsStunned => stunned;



    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        if (target == null)
        {
            var p = GameObject.FindGameObjectWithTag("Player");
            if (p != null) target = p.transform;
        }
        
        home = rb.position;
        PickNewWanderTarget();
    }
    
    
    void WanderSteer()
    {
        Vector2 desiredVel = (wanderTarget - rb.position).normalized * wanderSpeed;
        Vector2 steer = desiredVel - rb.linearVelocity;

        // limit acceleration so it doesn't snap
        steer = Vector2.ClampMagnitude(steer * steerStrength, maxAccel);

        rb.AddForce(steer, ForceMode2D.Force);
    }
    
    
    void PickNewWanderTarget()
    {
        Vector2 pos = rb.position;

        // Use current velocity as "forward". If stopped, pick a gentle default.
        Vector2 forward = rb.linearVelocity.sqrMagnitude > 0.2f ? rb.linearVelocity.normalized : Vector2.right;

        for (int i = 0; i < 20; i++)
        {
            // Mix forward direction with random direction to bias motion
            Vector2 randDir = Random.insideUnitCircle.normalized;
            Vector2 dir = (forward * forwardBias + randDir * (1f - forwardBias)).normalized;

            float dist = Random.Range(minTargetDistance, wanderRadius);
            Vector2 candidate = home + dir * dist;

            // Reject targets inside walls
            if (Physics2D.OverlapCircle(candidate, 0.2f, obstacleMask) != null)
                continue;

            // Reject targets behind walls
            Vector2 to = candidate - pos;
            float d = to.magnitude;
            if (d < 0.2f) continue;

            var hit = Physics2D.Raycast(pos, to / d, d, obstacleMask);
            if (hit.collider != null) continue;

            wanderTarget = candidate;

            // Don’t force a new target too soon; we’ll mainly repick on arrival/stuck
            nextWanderTime = Time.time + wanderInterval;
            return;
        }

        // Fallback
        wanderTarget = home;
        nextWanderTime = Time.time + 0.5f;
    }


    void Wander()
    {
        // Slowly move home toward current position so it drifts through the level
        // home = Vector2.Lerp(home, rb.position, homeFollow * Time.fixedDeltaTime * 60f);

        float distToTarget = Vector2.Distance(rb.position, wanderTarget);

        // Repick mainly when we arrived, or we’re stuck, or timer elapsed as a backup
        bool arrived = distToTarget <= arriveDistance;
        bool timeUp = Time.time >= nextWanderTime;

        WanderSteer();

        // Stuck detection
        if (rb.linearVelocity.magnitude < stuckSpeedThreshold)
        {
            stuckTimer += Time.fixedDeltaTime;
        }
        else
        {
            stuckTimer = 0f;
        }

        bool stuck = stuckTimer >= stuckTimeToRepath;

        if (arrived || stuck || timeUp)
        {
            PickNewWanderTarget();
            stuckTimer = 0f;
        }
    }

    public void Stun(float seconds)
    {
        if (!gameObject.activeInHierarchy) return;
        CancelInvoke(nameof(EndStun));

        stunned = true;
        rb.linearVelocity = Vector2.zero;

        Invoke(nameof(EndStun), seconds);
    }

    private void EndStun()
    {
        stunned = false;
    }

    void FixedUpdate()
    {
        if (stunned) { rb.linearVelocity = Vector2.zero; return; }

        var vision = GetComponent<EnemyVision2D>();
        if (vision != null && !vision.CanSeePlayer)
        {
            Wander();
            return;
        }

        if (target == null) return;


        Vector2 toTarget = (Vector2)(target.position - transform.position);
        
        float dx = toTarget.x;
        if (Mathf.Abs(dx) > approachDeadZone)
            LastApproachXDir = Mathf.Sign(dx);
        
        float dist = toTarget.magnitude;

        if (dist <= stopDistance)
        {
            rb.linearVelocity = Vector2.zero;
            return;
        }

        Vector2 dir = toTarget / Mathf.Max(dist, 0.0001f);
        rb.linearVelocity = dir * moveSpeed;
    }
}