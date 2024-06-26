using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Key : MonoBehaviour , IInteractable
{

    Transform KeyFollowPoint;
    AudioSource m_AudioSource;
    [SerializeField] AudioClip KeyCollect;
    [SerializeField] float BounceSpeed = 5;
    [SerializeField] float BounceHeight = 0.5f;
    [SerializeField] float MoveToSpeed = 1;
    bool bInteracted = false;
    Coroutine mcr_Follow;

    public void Awake()
    {
        
    }
    public void OnInteract(GameObject Interactor)
    {
        ItemStorageScript PlayerInv = Interactor.GetComponent<ItemStorageScript>();
        if (PlayerInv != null && !bInteracted )
        {
            bInteracted = true;
            m_AudioSource = Interactor.GetComponent<AudioSource>();
            if( m_AudioSource != null)
            {
                m_AudioSource.PlayOneShot(KeyCollect,1);
            }
            PlayerInv.AddItem(gameObject);
            KeyFollowPoint = Interactor.GetComponent<PlayerController>().KeyFollowPoint;
            //Play Collect Audio
            if (KeyFollowPoint) { mcr_Follow = StartCoroutine(FollowPlayer(KeyFollowPoint)); }
        }
    }

    IEnumerator FollowPlayer(Transform KeyFollow)
    {
        while (true)
        {
            transform.localPosition = Vector3.Lerp(transform.position,KeyFollow.position + new Vector3(0,Mathf.Sin(BounceSpeed * Time.time) * BounceHeight), MoveToSpeed *Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
    }


    public void OnUse(GameObject U)
    {
        GetComponent<Collider2D>().enabled = false;
        Debug.Log("Used Key");
        StopAllCoroutines();
        StopCoroutine(mcr_Follow);
        mcr_Follow = null;
        StartCoroutine(MoveToLocation(U.gameObject.transform));
    }

    IEnumerator MoveToLocation(Transform Location)
    {
        MoveToSpeed = 3;
        while(transform.position != Location.position)
        {
        transform.localPosition = Vector3.Lerp(transform.position, Location.position , MoveToSpeed * Time.deltaTime);
        yield return new WaitForFixedUpdate();
        }
    }
}