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
    AudioSource m_AudioSource;
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
        m_AudioSource.Play();
        GameObject Ball = Instantiate(Cannonball, FirePoint.transform.position, transform.rotation);
        Ball.GetComponent<Projectile>().Direction = ShootDir;
    }

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
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
