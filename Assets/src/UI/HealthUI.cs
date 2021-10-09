using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    public Image Heart1;
    public Image Heart2;
    public Image Heart3;

    public Sprite HealthyHeart;
    public Sprite HurtHeart;

    private void Update()
    {
        var playerHealth = PlayerController.Instance.Health;

        switch(playerHealth)
        {
            case 0:
                Heart1.sprite = HurtHeart;
                Heart2.sprite = HurtHeart;
                Heart3.sprite = HurtHeart;
                break;
            case 1:
                Heart1.sprite = HurtHeart;
                Heart2.sprite = HurtHeart;
                Heart3.sprite = HealthyHeart;
                break;
            case 2:
                Heart1.sprite = HurtHeart;
                Heart2.sprite = HealthyHeart;
                Heart3.sprite = HealthyHeart;
                break;
            case 3:
                Heart1.sprite = HealthyHeart;
                Heart2.sprite = HealthyHeart;
                Heart3.sprite = HealthyHeart;
                break;
        }
    }
}
