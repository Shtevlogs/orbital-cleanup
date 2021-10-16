using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PositionIndicatorUI : MonoBehaviour
{
    public float Angle = 0f;

    public Transform PlayerImage;

    private void Update()
    {
        var transform = GetComponent<RectTransform>();

        var rotation = Quaternion.Euler(0f, 0f, Angle);

        var direction = (Vector2)(rotation * Vector2.right);

        direction = new Vector2(direction.x * Camera.main.aspect, direction.y * 1.75f / Camera.main.aspect);

        var halfWidth = Mathf.Min(Camera.main.scaledPixelWidth / 3.5f, Camera.main.scaledPixelHeight / 3.5f);

        transform.rotation = rotation;
        transform.anchoredPosition = direction * halfWidth;

        if(PlayerController.Instance != null)
        {
            PlayerImage.rotation = PlayerController.Instance.transform.rotation;
        }
    }
}
