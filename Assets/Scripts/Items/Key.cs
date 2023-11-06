using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Key : MonoBehaviour , IInteractable
{
    public void OnInteract(GameObject Interactor)
    {
        PlayerController PlayerRef = Interactor.GetComponent<PlayerController>();
        if (PlayerRef != null )
        {
            Destroy(gameObject);
            PlayerRef.KeyHeld = true;
        }
    }
}
