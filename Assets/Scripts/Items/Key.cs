using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Key : MonoBehaviour , IInteractable
{
    public void OnInteract(GameObject Interactor)
    {
        ItemStorageScript PlayerInv = Interactor.GetComponent<ItemStorageScript>();
        if (PlayerInv != null )
        {
            PlayerInv.AddItem(gameObject);
        }
    }
}
