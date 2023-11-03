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
                player.transform.position += CalcDistanceFromWall(-1);
            }
            else if (collision.IsTouching(T_Right) && controller.IsPlayerRising())
            {
                Debug.Log("Right");

                player.transform.position += CalcDistanceFromWall(1);

            }
            else if (collision.IsTouching(T_Bottom))
            {
               controller.GroundCheck();
            }
        }

    }

    Vector3 CalcDistanceFromWall(int Direction)
    {
        RaycastHit2D Ray = Physics2D.Raycast(RayStart.transform.position, new Vector2(100 * Direction, 0), 1, m_LayerMask);
        Vector2 Dist = new Vector2(-Direction*Ray.distance, 0);

        return Dist;
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
