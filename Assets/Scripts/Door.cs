using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour , IInteractable
{
    private Collider2D m_Collder;
    [SerializeField] GameObject DesiredItem;
    [SerializeField] GameObject LockSprite;
    AudioSource m_AudioSource;
    [SerializeField] AudioClip DoorUnlock;
    [SerializeField] AudioClip DoorOpen;

    private void Awake()
    {
        m_Collder = GetComponentInChildren<Collider2D>();
        m_AudioSource = GetComponent<AudioSource>();
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
        m_AudioSource.PlayOneShot(DoorUnlock);
        LockSprite.SetActive(false);
        Destroy(DesiredItem);
        yield return new WaitForSecondsRealtime(0.4f);
        m_AudioSource.PlayOneShot(DoorOpen);
        enabled = false;
        yield return new WaitForSecondsRealtime(1f);
        Destroy(gameObject);
    }

    public void OnUse(GameObject U)
    {
    }

}
