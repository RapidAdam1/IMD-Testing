using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
   
    Collider2D m_collider;
    HealthComponent Health;
    bool SpikeActive = false;
    void Start()
    {
       m_collider = GetComponent<Collider2D>();
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Health == null)
        {
            Health = collision.GetComponentInParent<HealthComponent>();
            if (SpikeActive && Health != null)
            {
                Health.ApplyDamage(10);
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        Health = null;
    }
    private void ActivateSpike()
    {
        if(Health != null)
        {
            Health.ApplyDamage(10);
        }
        SpikeActive = true;
    }
    private void DeactivateSpike() 
    { 
        SpikeActive = false;
    }
}
