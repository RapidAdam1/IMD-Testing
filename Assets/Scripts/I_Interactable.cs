using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  interface IInteractable
{
    public void OnInteract(GameObject Interactor);

    public void OnUse(GameObject UsedOn);
}
