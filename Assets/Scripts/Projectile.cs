using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float DamagePercentage;

    Collider2D Collider;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        HealthComponent Health = collision.gameObject.GetComponent<HealthComponent>();
        if (Health)
            Health.ApplyDamage(DamagePercentage);
        Destroy(gameObject);
    }
}
