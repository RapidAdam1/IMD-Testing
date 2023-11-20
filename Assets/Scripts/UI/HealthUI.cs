using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class HealthUI : MonoBehaviour
{

    List<Image> Images = new List<Image>();
    [SerializeField] Sprite HealthFull;
    [SerializeField] Sprite HealthEmpty;



    private void Awake()
    {
        foreach (Image I in GetComponentsInChildren<Image>())
        {
            Images.Add(I);
        }
        SetHealthPercent(10,10);
    }

    public void SetHealthPercent(float CurrentHealth, float MaxHealth)
    {

        int Hearts = (int)((CurrentHealth / MaxHealth) * 10);
        int FilledHearts = 0;
        foreach (Image I in Images) 
        {
            if (FilledHearts < Hearts)
                I.sprite = HealthFull;
            else
            {
                I.sprite = HealthEmpty;
            }
            FilledHearts ++;
        }
    }
}
