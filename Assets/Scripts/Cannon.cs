using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{
    [SerializeField] GameObject Cannonball;
    [SerializeField] GameObject FirePoint;
    Vector2 FireDirection;
    private void Start()
    {
        FireDirection = Vector2.left + new Vector2(transform.rotation.x,transform.rotation.y);
        Debug.Log(FirePoint);
    }

    public void FireCannonBall()
    {
        Instantiate(Cannonball);
    }

    private void Update()
    {
        FireCannonBall();
    }
}
