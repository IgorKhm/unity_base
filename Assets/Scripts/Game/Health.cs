using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    public int maxHP = 5;
    public static event Action<GameObject> AnyDeath;

    public int CurrentHP { get; private set; }

    void Awake()
    {
        CurrentHP = maxHP;
    }

    public void TakeDamage(int amount)
    {
        if (CurrentHP <= 0) return;

        CurrentHP -= amount;
        if (CurrentHP <= 0)
        {
            CurrentHP = 0;
            AnyDeath?.Invoke(gameObject);
            Destroy(gameObject);
        }
    }
}