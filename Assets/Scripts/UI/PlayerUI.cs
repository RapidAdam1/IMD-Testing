using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;
using static UnityEngine.UI.Image;

public class PlayerUI : MonoBehaviour
{

    HealthUI HealthCounter;
    Text CoinCount;

    [SerializeField] float ScreenShakeIntensity = 1.0f;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject CoinM;
    [SerializeField] Image DeathImage;
    [SerializeField] GameObject Shakeable;

    private void Awake()
    {
        HealthComponent Health = Player.GetComponent<HealthComponent>();
        Health.OnHealthIncreased += UpdateHealth;
        Health.OnHealthDecreased += UpdateHealth;

        DeathImage.enabled = false;

        HealthCounter = GetComponentInChildren<HealthUI>();
        CoinManager CoinControl = CoinM.GetComponent<CoinManager>();
        CoinControl.OnCoinUpdate += UpdateCoins;
        CoinCount = GetComponentInChildren<Text>();
    }

    void UpdateCoins(int NewScore)
    {
        CoinCount.text = "Coins: " + NewScore.ToString();
    }
 
    public void UpdateHealth(float Current)
    {
        HealthCounter.SetHealthPercent(Current,Player.GetComponent<HealthComponent>().MaxHealth);
    }

    public void DoUIShake(float Duration) { StartCoroutine(ScreenShake(Duration,ScreenShakeIntensity)); }

    public void Dead()
    {
        Shakeable.SetActive(false);
        DeathImage.enabled = true;
    }

    IEnumerator ScreenShake(float Duration,float Intensity)
    {
        Debug.Log("StartedScreenShake");
        Vector3 Origin = transform.position;
        float ElapsedTime = 0;
        while (ElapsedTime < Duration)
        {
            ElapsedTime += Time.deltaTime;
            Shakeable.transform.position = Random.insideUnitSphere * Intensity + Origin;
            yield return new WaitForFixedUpdate() ;
        }
        Debug.Log("EndScreenShake");

        transform.position = Origin;
        yield break;
    }
}