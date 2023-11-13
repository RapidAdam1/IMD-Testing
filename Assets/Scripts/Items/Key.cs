using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Key : MonoBehaviour , IInteractable
{

    Transform KeyFollowPoint;
    [SerializeField] float BounceSpeed = 5;
    [SerializeField] float BounceHeight = 0.5f;
    [SerializeField] float MoveToSpeed = 1;
    public void OnInteract(GameObject Interactor)
    {
        ItemStorageScript PlayerInv = Interactor.GetComponent<ItemStorageScript>();
        if (PlayerInv != null )
        {
            PlayerInv.AddItem(gameObject);
            KeyFollowPoint = Interactor.GetComponent<PlayerController>().KeyFollowPoint;

            if (KeyFollowPoint) { StartCoroutine(FollowPlayer(KeyFollowPoint)); }
            
        }
    }

    IEnumerator FollowPlayer(Transform KeyFollow)
    {
        while (true)
        {
            transform.localPosition = Vector3.Lerp(transform.position,KeyFollow.position + new Vector3(0,Mathf.Sin(BounceSpeed * Time.time) * BounceHeight), MoveToSpeed *Time.deltaTime);
            yield return new WaitForFixedUpdate();
        }
        yield return null;
    }
}
