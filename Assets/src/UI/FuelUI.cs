using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FuelUI : MonoBehaviour
{
    public RectTransform FuelLevelDisplay;
    public Image FuelImage;
    public Color FuelFullColor;
    public Color FuelHalfColor;
    public Color FuelEmptyColor;

    public float LowFuelYCoord;

    private void Update()
    {
        var fuelLevel = Mathf.Clamp01(PlayerController.Instance.FuelLevel);

        var fuelYCoord = (1f - fuelLevel) * LowFuelYCoord;

        var fuelColorPercent = Mathf.Clamp01((fuelLevel % 0.5f) * 2f);
        var fuelColor = fuelLevel > 0.5f ? 
            (fuelColorPercent * FuelFullColor + (1f - fuelColorPercent) * FuelHalfColor) : 
            (fuelColorPercent * FuelHalfColor + (1f - fuelColorPercent) * FuelEmptyColor);

        FuelLevelDisplay.anchoredPosition = new Vector2(FuelLevelDisplay.anchoredPosition.x, fuelYCoord);
        FuelImage.color = fuelColor;
    }
}
