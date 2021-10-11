using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TimerUI : MonoBehaviour
{
    private TextMeshProUGUI text;

    private void Start()
    {
        text = GetComponentInChildren<TextMeshProUGUI>();
        GetComponent<Timer>().OnTimerTick += _onTimerTick;
    }

    private void _onTimerTick(int time)
    {
        text.text = "" + time;
        text.color = time >= 10 ? Color.white : Color.red;
    }
}
