using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, IInteractable
{
    [SerializeField] float HealthToAdd = 10;
    public virtual void OnInteract(GameObject Interactor) 
    { 
        Interactor.GetComponent<HealthComponent>().AddHealth(10);
        Destroy(gameObject);    
    }
}
