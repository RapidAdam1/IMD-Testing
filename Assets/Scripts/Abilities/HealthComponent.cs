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
    public event Action<bool> OnHealthChangeVFX;


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
        OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
        OnHealthChangeVFX?.Invoke(false);

    }

    public void AddHealth(float Health)
    {
        Mathf.Clamp(CurrentHealth+=Health, 0, MaxHealth);
        OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
        OnHealthChangeVFX?.Invoke(true);
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
