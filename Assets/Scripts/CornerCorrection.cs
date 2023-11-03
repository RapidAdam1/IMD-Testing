using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerCorrection : MonoBehaviour
{
    [SerializeField] Collider2D mainColl;

    [SerializeField] Collider2D T_Left;
    [SerializeField] Collider2D T_Right;
    PlayerController controller;
    [SerializeField] Collider2D T_Bottom;
    [SerializeField] GameObject player;

    private void Awake()
    {
        controller = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            if (collision.IsTouching(T_Left) && controller.IsPlayerRising())
            {
                player.transform.position += Vector3.right / 3;
            }
            else if (collision.IsTouching(T_Right) && controller.IsPlayerRising())
            {
                player.transform.position += Vector3.left / 3;

            }
            else if (collision.IsTouching(T_Bottom))
            {
               controller.GroundCheck();
            }
        }

    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground")
        {
            controller.StartFall();
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Ground")
        {
            controller.OnGrounded();
        }
    }
    public void EnableMainCollider(bool IsEnabled) {mainColl.enabled = IsEnabled;}
}
