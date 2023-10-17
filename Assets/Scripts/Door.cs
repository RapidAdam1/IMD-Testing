using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour , IInteractable
{
    private Collider2D m_Collder;

    private void Awake()
    {
        m_Collder = GetComponentInChildren<Collider2D>();
    }

    public void OnInteract(GameObject Interactor)
    {
        PlayerController PlayerRef = Interactor.GetComponent<PlayerController>();
        if (PlayerRef != null && PlayerRef.KeyHeld) 
        {
            PlayerRef.KeyHeld = false;
            m_Collder.gameObject.SetActive(false);
        }
    }
}
