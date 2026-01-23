using System.Collections;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public int maxHP = 6;
    public float invulnSeconds = 0.75f;

    public int CurrentHP { get; private set; }

    private bool invulnerable;
    private SpriteRenderer[] renderers;
    private bool debugKnockback = true;
    
    public float knockSpeedX = 10f;
    public float knockSpeedY = 4f;
    
    void Awake()
    {
        CurrentHP = maxHP;
        renderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void TakeDamage(int amount, float xDir)
    {
        if (invulnerable || CurrentHP <= 0) return;
        
        if (debugKnockback)
        {
            Debug.Log($"[DMG] amount={amount} xDirIn={xDir} playerPos={transform.position}");
        }

        CurrentHP -= amount;

        ApplyKnockbackX(xDir);

        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            Die();
            return;
        }

        StartCoroutine(InvulnRoutine());
    }

    private void ApplyKnockbackX(float xDir)
    {
        var rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        float s = Mathf.Sign(xDir);
        if (s == 0f) s = 1f;

        // Tune these

        rb.linearVelocity = new Vector2(s * knockSpeedX, knockSpeedY);
        GetComponent<PlayerController2D>()?.Stun(0.15f);
    }


    private IEnumerator InvulnRoutine()
    {
        invulnerable = true;

        // quick “blink” feedback (placeholder-friendly)
        float t = 0f;
        while (t < invulnSeconds)
        {
            SetRenderersEnabled(false);
            yield return new WaitForSecondsRealtime(0.08f);
            SetRenderersEnabled(true);
            yield return new WaitForSecondsRealtime(0.08f);
            t += 0.16f;
        }

        SetRenderersEnabled(true);
        invulnerable = false;
    }

    private void SetRenderersEnabled(bool on)
    {
        if (renderers == null) return;
        foreach (var r in renderers)
            if (r != null)
                r.enabled = on;
    }
    
    private void Die()
    {
        Debug.Log("PLAYER DIED");

        var gm = FindFirstObjectByType<GameManager>();
        if (gm != null) gm.Lose();
    }

    
    
}