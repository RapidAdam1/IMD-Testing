using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
   
    HealthComponent Health;
    bool SpikeActive = false;
    [SerializeField] float Damage = 10;

    void Start()
    {
    }
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(Health == null)
        {
            Health = collision.GetComponentInParent<HealthComponent>();
            if (SpikeActive && Health != null)
            {
                Health.ApplyDamage(Damage);
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
            //Play Spike Extend Audio
            Health.ApplyDamage(Damage);
        }
        SpikeActive = true;
    }
    private void DeactivateSpike() 
    { 
        //Play Spike Retract Audio
        SpikeActive = false;
    }
}
