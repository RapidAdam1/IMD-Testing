using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CornerCorrection : MonoBehaviour
{
    PlayerController PC;
    [SerializeField] Collider2D MainCollider;
    [SerializeField] Collider2D LeftCollider;
    [SerializeField] Collider2D RightCollider;

    private void Awake()
    {
       PC = GetComponentInParent<PlayerController>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground" && !PC.m_collider.enabled && PC.m_rb.velocity.y > 1)
        {
            if (collision.IsTouching(LeftCollider))
            {
                PC.m_rb.AddForce(Vector2.right * 100);

                StartCoroutine(DelayCollider());
            }
            else if(collision.IsTouching(RightCollider)) 
            {
                PC.m_rb.AddForce(Vector2.left * 100);
                StartCoroutine(DelayCollider());
                
            }
        }
        else if (collision.tag =="Ground")
        {
            PC.m_collider.enabled = true;
        }
    }

    IEnumerator DelayCollider()
    {
        yield return new WaitForSeconds(0.05f);
        PC.m_collider.enabled = true;
    }
}
