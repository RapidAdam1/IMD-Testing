using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CornerCorrection : MonoBehaviour
{
    [SerializeField] Collider2D T_Left;
    [SerializeField] Collider2D T_Right;

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

    }

    IEnumerator DelayCollider()
    {
        yield return new WaitForSeconds(0.05f);
        GetComponentInParent<PlayerController>().EnabledCollider(true);
    }
}
