using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeSlowScript : MonoBehaviour
{
    [SerializeField][Range(0.5f, 4f)] float SlowLength = 1;
    [SerializeField] [Range(0.2f, 0.9f)] float SlowedTimeScale;
    [SerializeField] float CooldownTime =1;
    
    [SerializeField] AudioClip SlowInSFX;
    [SerializeField] AudioClip SlowOutSFX;

    AudioSource m_AudioSource;
    float CurrentTimeScale = 1;
    bool bIsSlowed;
    public event Action<float,bool> OnTimeSlowed;

    private void Awake()
    {
        m_AudioSource = GetComponent<AudioSource>();
    }
    public void SlowTime()
    {
        if (bIsSlowed)
            return;
        //Play Slow Time Sounds
        StartCoroutine(SlowTimeRemaining(SlowLength));
    }

    IEnumerator SlowTimeRemaining(float TimeToBeSlowed)
    {
        bIsSlowed = true;
        Time.timeScale = SlowedTimeScale;
        OnTimeSlowed?.Invoke(SlowLength,true);
        m_AudioSource.PlayOneShot(SlowInSFX, 1);
        yield return new WaitForSecondsRealtime(TimeToBeSlowed);

        m_AudioSource.PlayOneShot(SlowOutSFX, 1);

        CurrentTimeScale = SlowedTimeScale;
        while (CurrentTimeScale <= 1)
        {
            CurrentTimeScale += 0.05f;
            Time.timeScale = CurrentTimeScale;
            yield return new WaitForFixedUpdate();
        }
        Time.timeScale = 1;
        OnTimeSlowed?.Invoke(SlowLength,false);
        StartCoroutine(Cooldown(CooldownTime));
        yield break;
    }

    IEnumerator Cooldown(float CooldownTime)
    {
        yield return new WaitForSeconds(CooldownTime);
        bIsSlowed = false;
    }
}
