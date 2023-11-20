using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    [SerializeField] float MoveSpeed;
    [SerializeField] float DamagePercentage;
    Rigidbody2D rb;
    public Vector3 Direction;
    Collider2D Collider;


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Player" || collision.tag == "Ground")
        {
            HealthComponent Health = collision.GetComponentInParent<HealthComponent>();
            if (Health)
                Health.ApplyDamage(DamagePercentage);
            Destroy(gameObject);
        }
    }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();

    }

    private void FixedUpdate()
    {
        transform.position += Direction * MoveSpeed;
         rb.AddForce(Direction * MoveSpeed);
    }
}
