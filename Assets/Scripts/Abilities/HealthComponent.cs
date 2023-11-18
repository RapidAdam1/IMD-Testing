using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{ 
    [SerializeField] float MaxHealth = 10.0f;

    float CurrentHealth;
    [SerializeField] bool IsPlayer;

    public event Action<bool> OnDeath;
    public event Action<float,float> OnHealthChange;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }
    public float GetHealthRatio() { return CurrentHealth / MaxHealth; }
    public void ApplyDamage(float DamagePercent)
    {
        float Damage = MaxHealth / DamagePercent;
        CurrentHealth -= Damage;
        if (CurrentHealth <= 0) 
        {
            CurrentHealth = 0;
            Death(); 
        }
        OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
    }

    public void AddHealth(float HealthPercent)
    {
        float HealthToAdd = (MaxHealth/100) * HealthPercent;
        Mathf.Clamp(CurrentHealth+=HealthToAdd, 0, MaxHealth);
        OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
    }

    protected virtual void Death()
    {
        if(IsPlayer)
        {
            OnDeath?.Invoke(true);
        }
        else
        {
            OnDeath?.Invoke(false);
            Destroy(gameObject);
        }
    }

}
