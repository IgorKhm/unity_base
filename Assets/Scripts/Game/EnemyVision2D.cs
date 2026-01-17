using UnityEngine;

public class EnemyVision2D : MonoBehaviour
{
    public Transform player;
    public float viewDistance = 12f;

    // What can block vision (Ground/Platforms/Walls)
    public LayerMask obstacleMask;

    // Optional: only “see” player if inside this angle (0 = disabled)
    [Range(0f, 180f)] public float fovDegrees = 0f;

    public bool CanSeePlayer { get; private set; }

    void Start()
    {
        if (player == null)
        {
            var go = GameObject.FindGameObjectWithTag("Player");
            if (go) player = go.transform;
        }
    }

    void Update()
    {
        if (player == null) { CanSeePlayer = false; return; }

        Vector2 toPlayer = player.position - transform.position;
        float dist = toPlayer.magnitude;

        if (dist > viewDistance)
        {
            CanSeePlayer = false;
            return;
        }

        // Optional FOV cone (only if you track facing direction)
        if (fovDegrees > 0f)
        {
            // Example: assume enemy “faces” along its local right axis
            Vector2 forward = transform.right;
            float ang = Vector2.Angle(forward, toPlayer);
            if (ang > fovDegrees * 0.5f)
            {
                CanSeePlayer = false;
                return;
            }
        }

        Vector2 dir = toPlayer / Mathf.Max(dist, 0.0001f);

        // Raycast against obstacles only. If it hits an obstacle, vision is blocked.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, obstacleMask);
        CanSeePlayer = (hit.collider == null);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(transform.position, viewDistance);
    }
}