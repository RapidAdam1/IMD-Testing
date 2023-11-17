using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{ 
    [SerializeField] float MaxHealth = 10.0f;

    float CurrentHealth;
    float DamageForgiveness = 1.0f;
    [SerializeField] bool IsPlayer;

    public event Action<float,float> OnHealthChange;

    private void Awake()
    {
        CurrentHealth = MaxHealth;
    }
    public void ApplyDamage(float DamagePercent)
    {
        float Damage = MaxHealth / DamagePercent;
        if (Damage > CurrentHealth && CurrentHealth != DamageForgiveness) { CurrentHealth = DamageForgiveness; }
        else 
        {
            CurrentHealth -= Damage;
            if (CurrentHealth <= 0) 
            {
                CurrentHealth = 0;
                OnDeath(); 
            }
            OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
        }
    }

    public void AddHealth(float HealthPercent)
    {
        Mathf.Clamp(CurrentHealth+=MaxHealth/HealthPercent, 0, MaxHealth);
        OnHealthChange?.Invoke(CurrentHealth, MaxHealth);
    }

    protected virtual void OnDeath()
    {
        if(IsPlayer)
        {

        }
        else
        {
            Destroy(gameObject);
        }
    }

}
