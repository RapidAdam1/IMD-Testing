using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour, IInteractable
{
    [SerializeField] float HealthPercentToAdd = 10;
    public virtual void OnInteract(GameObject Interactor) 
    { 
        HealthComponent HealthComp = Interactor.GetComponent<HealthComponent>();
        if (HealthComp.GetHealthRatio() == 1)
            return;
        HealthComp.AddHealth(HealthPercentToAdd);
        Destroy(gameObject);
    }

    public void OnUse(GameObject U)
    {
        
    }
}
