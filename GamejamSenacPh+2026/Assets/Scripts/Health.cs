using System;
using UnityEngine;

public class Health : MonoBehaviour
{
    [SerializeField] float maxHealth;
    private Action onDie;
    float currentHealth;

    public float CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public float MaxHealth { get => maxHealth; set => maxHealth = value; }
    public Action OnDie { get => onDie; set => onDie = value; }

    void Awake()
    {
        CurrentHealth = MaxHealth;
    }
    public void TakeDamage(float ammount)
    {
        if (ammount > 0)
            CurrentHealth -= ammount;
        else Heal(ammount);

        if(CurrentHealth < 0) 
        {
            OnDie?.Invoke();
        }
    }
    public void Heal(float ammount) 
    {
        CurrentHealth += ammount;

    }
   

    
    
}
