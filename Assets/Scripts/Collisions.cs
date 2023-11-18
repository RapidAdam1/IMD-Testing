using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collisions : MonoBehaviour
{
    [SerializeField] Collider2D mainColl; public Collider2D GetMainColl() { return mainColl; }

    [SerializeField] Collider2D T_Left;
    [SerializeField] Collider2D T_Right;
    PlayerController controller;
    [SerializeField] Collider2D T_Bottom;
    [SerializeField] GameObject RayStart;
    [SerializeField] LayerMask m_LayerMask;
    [SerializeField] Transform m_CastPosition;
    [SerializeField] GameObject player;
    public bool isGrounded;

    private void Awake()
    {
        controller = GetComponentInParent<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag != "Ground")
            return;
        if (collision.IsTouching(T_Left))
        {
            player.transform.position += Vector3.right/3;
            StartCoroutine(WaitThenEnableCollider());
        }
        else if (collision.IsTouching(T_Right))
        {
            player.transform.position += Vector3.left/3;
        } 
   }

    Vector3 CornerCorrectPlacement(Vector2 RayCastDirection)
    {
        RaycastHit2D Ray = Physics2D.Raycast(RayStart.transform.position, RayCastDirection, 10, m_LayerMask);
        Debug.Log(Ray.distance);
        Vector3 NewPos = new Vector3(Ray.distance * RayCastDirection.x *-1, 0, 0);
        return NewPos;
    }

    IEnumerator WaitThenEnableCollider()
    {
        yield return new WaitForSeconds(0.1f);
        EnableMainCollider(true);
        yield break;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.collider.tag == "Ground" && !GroundCheck())
        {
            EnableMainCollider(false);
            controller.StartFall();
        }
    }
    public void OnCollisionEnter2D(Collision2D collision)
    {

        if (collision.collider.tag == "Ground")
        {
            GroundCheck();
        }
    }

    public void OnGrounded()
    {
        EnableMainCollider(true);
        controller.OnGrounded();
    }

    bool GroundCheck()
    {
        isGrounded = Physics2D.BoxCast(m_CastPosition.position, new Vector2(.9f, 0.2f), 0, Vector2.zero, 0, m_LayerMask);
        if (isGrounded)
        {
            OnGrounded();
        }
        return isGrounded;
    }


    public bool CoyoteCollisionCheck()
    {
        return !Physics2D.BoxCast(m_CastPosition.position + new Vector3(0, 1f), new Vector2(1.5f, 1.3f), 0, Vector2.zero, 0, m_LayerMask);
    }
    public bool CanJumpBuffer()
    {
        bool CanJump = Physics2D.Linecast(m_CastPosition.position, m_CastPosition.position - Vector3.up, m_LayerMask);
        return CanJump;
    }

    public void EnableMainCollider(bool IsEnabled) {mainColl.enabled = IsEnabled;}
}
