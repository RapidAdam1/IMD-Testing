using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpikeTrap : MonoBehaviour
{
   
    Collider2D m_collider;
    HealthComponent Health;
    void Start()
    {
       m_collider = GetComponent<Collider2D>();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (Health = collision.gameObject.GetComponent<HealthComponent>()) ;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (Health && m_collider == true)
        {
            Debug.Log("HIT");
        }
    }

    private void ActivateSpike()
    {
        m_collider.enabled = true ;
    }
    private void DeactivateSpike() 
    { 
        m_collider.enabled = false;
    }
}
