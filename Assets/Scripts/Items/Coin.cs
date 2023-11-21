using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Coin : MonoBehaviour
{
    public event Action<Coin> OnCoinPickup;
    AudioSource m_AudioSource;
    [SerializeField] AudioClip CoinCollectSound;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(m_AudioSource = collision.GetComponentInParent<AudioSource>())
        {
            m_AudioSource.PlayOneShot(CoinCollectSound, .7f);
        }
        OnCoinPickup?.Invoke(this);
    }
}
