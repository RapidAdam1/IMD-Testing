using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUI : MonoBehaviour
{

    HealthUI HealthCounter;
    Text CoinCount;

    [SerializeField] float ScreenShakeIntensity = 1.0f;
    [SerializeField] GameObject Player;
    [SerializeField] GameObject CoinM;

    private void Awake()
    {
        Player.GetComponent<HealthComponent>().OnHealthChange += UpdateHealth;
        HealthCounter = GetComponentInChildren<HealthUI>();
        CoinManager CoinControl = CoinM.GetComponent<CoinManager>();
        CoinControl.OnCoinUpdate += UpdateCoins;
        CoinCount = GetComponentInChildren<Text>();
    }


    private void Start()
    {
    }

    void UpdateCoins(int NewScore)
    {
        CoinCount.text = "Coins: " + NewScore.ToString();
    }
    public void UpdateHealth(float Current, float Max)
    {
        HealthCounter.SetHealthPercent(Current,Max);
    }

    public void DoUIShake(float Duration) { StartCoroutine(ScreenShake(Duration,ScreenShakeIntensity)); }

    IEnumerator ScreenShake(float Duration,float Intensity)
    {
        Debug.Log("StartedScreenShake");
        Vector3 Origin = transform.position;
        float ElapsedTime = 0;
        while (ElapsedTime < Duration)
        {
            ElapsedTime += Time.deltaTime;
            this.transform.position = Random.insideUnitSphere * Intensity + Origin;
            GUI.matrix = Matrix4x4.TRS(Origin*Intensity, Quaternion.identity, Vector3.one);
            yield return new WaitForFixedUpdate() ;
        }
        Debug.Log("EndScreenShake");

        transform.position = Origin;
        yield break;
    }
}
