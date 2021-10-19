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

    private int lastPlayerMaxHealth = 0;

    private void Update()
    {
        if(lastPlayerMaxHealth != PlayerController.Instance.MaxHealth)
        {
            _setVisiblilities(PlayerController.Instance.MaxHealth);
        }

        var playerHealth = PlayerController.Instance.Health;

        switch(playerHealth)
        {
            case 0:
                Heart1.sprite = HurtHeart;
                Heart2.sprite = HurtHeart;
                Heart3.sprite = HurtHeart;
                break;
            case 1:
                Heart1.sprite = HealthyHeart;
                Heart2.sprite = HurtHeart;
                Heart3.sprite = HurtHeart;
                break;
            case 2:
                Heart1.sprite = HealthyHeart;
                Heart2.sprite = HealthyHeart;
                Heart3.sprite = HurtHeart;
                break;
            case 3:
                Heart1.sprite = HealthyHeart;
                Heart2.sprite = HealthyHeart;
                Heart3.sprite = HealthyHeart;
                break;
        }
    }

    private void _setVisiblilities(int maxHealth)
    {
        lastPlayerMaxHealth = maxHealth;

        switch (maxHealth)
        {
            case 3:
                Heart1.gameObject.SetActive(true);
                Heart2.gameObject.SetActive(true);
                Heart3.gameObject.SetActive(true);
                break;
            case 2:
                Heart1.gameObject.SetActive(true);
                Heart2.gameObject.SetActive(true);
                Heart3.gameObject.SetActive(false);
                break;
            default:
                Heart1.gameObject.SetActive(true);
                Heart2.gameObject.SetActive(false);
                Heart3.gameObject.SetActive(false);
                break;
        }
    }
}
