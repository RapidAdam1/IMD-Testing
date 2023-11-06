using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthComponent : MonoBehaviour
{ 
    [SerializeField] float MaxHealth = 100.0f;
    float CurrentHealth;
    float DamageForgiveness = 1.0f;

    public void ApplyDamage(float Damage)
    {
        if (Damage > CurrentHealth && CurrentHealth != DamageForgiveness) { CurrentHealth = DamageForgiveness; }
        else 
        {
            CurrentHealth -= Damage;
            if (CurrentHealth <= 0) 
            {
                CurrentHealth = 0;
                OnDeath(); 
            }
        }
    }

    public void AddHealth(float Health)
    {
        Mathf.Clamp(CurrentHealth+=Health, 0, MaxHealth);
    }

    protected virtual void OnDeath()
    {

    }

}
