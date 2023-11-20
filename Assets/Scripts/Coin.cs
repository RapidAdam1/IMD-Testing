using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public event Action<Coin> OnCoinPickup;
    
    private void OnTriggerEnter2D(Collider2D collision)
    {
        OnCoinPickup?.Invoke(this);
    }
}
