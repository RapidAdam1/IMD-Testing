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
    [SerializeField] GameObject RayStart;
    [SerializeField] LayerMask m_LayerMask;
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
                Debug.Log("LEFT");
                player.transform.position += CalcDistanceFromWall(Vector2.left);
            }
            else if (collision.IsTouching(T_Right) && controller.IsPlayerRising())
            {
                Debug.Log("Right");

                player.transform.position += CalcDistanceFromWall(Vector2.right);

            }
        }

    }

    Vector3 CalcDistanceFromWall(Vector2 Direction)
    {
        RaycastHit2D Ray = Physics2D.Raycast(RayStart.transform.position, Direction, 1, m_LayerMask);
        Debug.Log(-1 * Direction * new Vector2(Ray.distance, 0));
        return -1 * Direction *new Vector2(Ray.distance,0);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && !controller.GroundCheck())
        {
            EnableMainCollider(false);
            controller.StartFall();
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Ground")
        {
            controller.GroundCheck();
        }
    }
    public void EnableMainCollider(bool IsEnabled) {mainColl.enabled = IsEnabled;}
}
