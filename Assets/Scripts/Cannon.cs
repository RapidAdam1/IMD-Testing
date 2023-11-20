using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour
{

    enum FireDirection
    {
        Left,
        Right
    }
    [SerializeField] GameObject Cannonball;
    [SerializeField] GameObject FirePoint;
    [SerializeField] FireDirection FireDir;
    Vector2 ShootDir;
    bool bActive = true;

    public void FireCannonBall()
    {
        //Play Fire Cannon Sound
        switch (FireDir)
        {
            case FireDirection.Left:
                ShootDir = Vector2.left;
                break;
            case FireDirection.Right:
                ShootDir = Vector2.right;
                break;
        }
        GameObject Ball = Instantiate(Cannonball, FirePoint.transform.position, transform.rotation);
        Ball.GetComponent<Projectile>().Direction = ShootDir;
    }

    private void Awake()
    {
        //StartCoroutine(ShootCannon());
    }

    IEnumerator ShootCannon()
    {
        while (bActive) 
        {
            yield return new WaitForSeconds(1);
            FireCannonBall();
        }
    }
}
