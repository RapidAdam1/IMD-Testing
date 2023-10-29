using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CornerCorrection : MonoBehaviour
{
    [SerializeField] Collider2D T_Left;
    [SerializeField] Collider2D T_Right;

    [SerializeField] Collider2D C_Bottom;
    [SerializeField] Collider2D T_Top;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            if (collision.IsTouching(T_Left))
            {
                StartCoroutine(DelayCollider());
            }
            else if (collision.IsTouching(T_Right))
            {
                StartCoroutine(DelayCollider());

            }
        }
        else if (collision.tag == "Ground")
        {
        }
    }

    public void FallingCollider()
    {
        C_Bottom.enabled = true;
    }
    public void RisingCollider()
    {
        C_Bottom.enabled = false;
    }

    IEnumerator DelayCollider()
    {
        yield return new WaitForSeconds(0.05f);
        GetComponentInParent<PlayerController>().EnabledCollider(true);
    }
}
