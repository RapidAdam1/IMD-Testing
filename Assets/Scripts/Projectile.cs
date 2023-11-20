using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float DamagePercentage;

    Collider2D Collider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        HealthComponent Health = collision.gameObject.GetComponent<HealthComponent>();
        if (Health)
            Health.ApplyDamage(DamagePercentage);
        Destroy(gameObject);
    }
}
