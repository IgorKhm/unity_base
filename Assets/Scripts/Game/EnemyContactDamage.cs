using System.Collections;
using UnityEngine;

public class EnemyContactDamage : MonoBehaviour
{
    public int damage = 1;
    public float hitCooldown = 0.75f;

    private float nextHitTime;
    private Collider2D enemyCol;

    void Awake()
    {
        enemyCol = GetComponent<Collider2D>();
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (Time.time < nextHitTime) return;

        var ph = collision.collider.GetComponentInParent<PlayerHealth>();
        if (ph == null) return;
        Collider2D playerCol = collision.collider;

        var chaser = GetComponent<EnemyChaser2D>();
        float xDir = 1f;

        if (chaser != null)
            xDir = chaser.LastApproachXDir;

        var enemyCol = GetComponent<Collider2D>();

        ph.TakeDamage(damage, xDir);


        // Temporarily disable collisions between this enemy and the player's collider
        if (enemyCol != null && playerCol != null)
            StartCoroutine(IgnoreForSeconds(playerCol, hitCooldown));

        if (chaser != null)
        {
            GetComponent<EnemyChaser2D>()?.Stun(hitCooldown);
            GetComponent<EnemyWalker2D>()?.Stun(hitCooldown);
            chaser.Stun(hitCooldown);
        }
    }

    private IEnumerator IgnoreForSeconds(Collider2D playerCol, float seconds)
    {
        Physics2D.IgnoreCollision(enemyCol, playerCol, true);

        // If you pause the game via Time.timeScale, this would freeze.
        // For cooldown feel, that's usually fine; if you want it independent, use WaitForSecondsRealtime.
        yield return new WaitForSeconds(seconds);

        if (enemyCol != null && playerCol != null)
            Physics2D.IgnoreCollision(enemyCol, playerCol, false);
    }
}