using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraBehaviour : MonoBehaviour
{
    [SerializeField]
    private float lead = 1f;
    [SerializeField]
    private float transition = 0.2f;

    private Vector2 leadLocation = Vector2.zero;
    private Vector2 leadTargetLocation = Vector2.zero;

    [SerializeField]
    private Vector2 center = Vector2.zero;
    [SerializeField]
    private Vector2 maxBounds;

    [SerializeField]
    private float minZoom = 4f;

    private new Camera camera;

    private void Start()
    {
        camera = GetComponent<Camera>();
    }

    public void OnRightStick(InputAction.CallbackContext value)
    {
        leadTargetLocation = value.ReadValue<Vector2>() * lead;
    }

    private void Update()
    {
        var target = PlayerController.Instance.transform;

        leadLocation = Vector2.Lerp(leadLocation, leadTargetLocation, transition * Time.deltaTime);
        var leadPos = target.position + (Vector3)leadLocation;
        transform.position = new Vector3(
            Mathf.Clamp(leadPos.x, -maxBounds.x, maxBounds.x), 
            Mathf.Clamp(leadPos.y, -maxBounds.y, maxBounds.y),
            transform.position.z);

        camera.orthographicSize = Mathf.Clamp(((Vector2)transform.position - center).magnitude, minZoom, 100f);
    }
}
