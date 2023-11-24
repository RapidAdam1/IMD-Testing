using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{ 
    [SerializeField] public float MaxHealth = 10.0f;

    float CurrentHealth;
    [SerializeField] bool IsPlayer;

    public event Action OnDeath;
    public event Action<float> OnHealthIncreased;
    public event Action<float> OnHealthDecreased;


    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }
    public float GetHealthRatio() { return CurrentHealth / MaxHealth; }
    public void ApplyDamage(float Damage)
    {
        CurrentHealth -= Damage;
        if (CurrentHealth <= 0) 
        {
            CurrentHealth = 0;
            Death(); 
        }
        OnHealthDecreased?.Invoke(CurrentHealth);
    }

    public void AddHealth(float Health)
    {
        Mathf.Clamp(CurrentHealth+=Health, 0, MaxHealth);
        OnHealthIncreased?.Invoke(CurrentHealth);
    }

    protected virtual void Death()
    {
        OnDeath?.Invoke();
    }

}
