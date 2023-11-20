using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour
{
    int Coins = 0;
    public event Action<int> OnCoinUpdate;
    private void Awake()
    {
        foreach (Coin a in FindObjectsOfType<Coin>())
        {
            a.OnCoinPickup += CoinPickedUp;
        }

    }
    void CoinPickedUp(Coin coin)
    {
        coin.OnCoinPickup -= CoinPickedUp;
        Coins++;
        OnCoinUpdate?.Invoke(Coins);
        Destroy(coin.gameObject);
    }
}
