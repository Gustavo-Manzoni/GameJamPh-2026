using System;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100f;


    public float MaxHealth => maxHealth;
    public float CurrentHealth { get; private set; }
    public bool IsDead => CurrentHealth <= 0f;

    public event Action<float, float> OnHealthChanged; 
 
    public event Action<float> OnDamageTaken;
    public event Action OnDeath;

    private void Awake()
    {
        CurrentHealth = maxHealth;
    }

    public void TakeDamage(float amount)
    {
        if (IsDead || amount <= 0f) return;
    

       
        CurrentHealth = Mathf.Max(CurrentHealth - amount, 0f);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
        OnDamageTaken?.Invoke(amount);

        if (IsDead)
        {
            OnDeath?.Invoke();
        }
    }
    public void PayFlatCost(float amount)
    {
        if (amount <= 0f) return;

        CurrentHealth = Mathf.Max(CurrentHealth - amount, 1f);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }

    public void Heal(float amount, bool ignoreHealingDisabled = false)
    {
        if (amount <= 0f) return;

        CurrentHealth = Mathf.Min(CurrentHealth + amount, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
    [ContextMenu("Take 13 Damage")]
    void TakeThirteenDamage(){TakeDamage(13f);}
    [ContextMenu("Heal 5 ")]
     void HealFiveDamage(){Heal(5f);}
    public void SetMaxHealth(float newMax, bool healToFull = false)
    {
        maxHealth = newMax;
        CurrentHealth = healToFull ? maxHealth : Mathf.Min(CurrentHealth, maxHealth);
        OnHealthChanged?.Invoke(CurrentHealth, maxHealth);
    }
}
