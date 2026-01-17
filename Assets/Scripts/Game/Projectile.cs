using UnityEngine;

public class Projectile : MonoBehaviour
{
    public float speed = 18f;
    public float lifeTime = 2f;
    public int damage = 1;

    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Launch(Vector2 direction)
    {
        rb.linearVelocity = direction.normalized * speed;
        Destroy(gameObject, lifeTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Only damage enemies
        if (other.TryGetComponent<Health>(out var hp))
        {
            hp.TakeDamage(damage);
            Destroy(gameObject);
            return;
        }

        if (other.gameObject.layer == LayerMask.NameToLayer("Ground"))
            Destroy(gameObject);
    }
}