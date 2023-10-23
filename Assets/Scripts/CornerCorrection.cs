using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class CornerCorrection : MonoBehaviour
{

    [SerializeField] Collider2D MainCollider;
    [SerializeField] Collider2D LeftCollider;
    [SerializeField] Collider2D RightCollider;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Ground")
        {
            if(collision.IsTouching(LeftCollider)) 
            {
                Debug.Log("LeftSide");
            }
            else if(collision.IsTouching(RightCollider)) 
            {
                Debug.Log("RightSide");
            }
        }
    }
}
