using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour , IInteractable
{
    private Collider2D m_Collder;
    [SerializeField] GameObject DesiredItem;
    [SerializeField] GameObject LockSprite;

    private void Awake()
    {
        m_Collder = GetComponentInChildren<Collider2D>();
    }

    public void OnInteract(GameObject Interactor)
    {
        ItemStorageScript PlayerInv = Interactor.GetComponent<ItemStorageScript>();
        if (PlayerInv == null)
            return;
        GameObject Item = PlayerInv.GetItem(DesiredItem);
        if (Item != DesiredItem)
            return;

        PlayerInv.RemoveItem(DesiredItem,gameObject);
        StartCoroutine(OpenDoor());
    }

    IEnumerator OpenDoor()
    {

        yield return new WaitForSecondsRealtime(2);
        LockSprite.SetActive(false);
        Destroy(DesiredItem);
        Destroy(gameObject);
    }

    public void OnUse(GameObject U)
    {
    }

}
