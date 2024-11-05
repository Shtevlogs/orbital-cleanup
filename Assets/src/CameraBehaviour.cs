using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehaviour : MonoBehaviour
{
    public static CameraBehaviour Instance;

    private void Awake()
    {
        Instance = this;
    }

    [SerializeField]
    private float lead = 1f;
    [SerializeField]
    private float transition = 0.2f;

    private Vector2 leadLocation = Vector2.zero;
    private Vector2 leadTargetLocation = Vector2.zero;

    [SerializeField]
    private Vector2 center = Vector2.zero;
    [SerializeField]
    public Vector2 MaxBounds;

    public Transform Background;

    [SerializeField]
    private float minZoom = 4f;
    [SerializeField]
    private float maxZoom = 1000f;

    public bool ForceMaxZoom = false;

    [SerializeField]
    private new Camera camera;

    [SerializeField]
    private PositionIndicatorUI positionIndicator;

    private void Start()
    {
        var listeners = FindObjectsByType<AudioListener>(FindObjectsSortMode.None);
        if(listeners.Length == 1)
        {
            GetComponent<AudioListener>().enabled = true;
        }
    }

    /*

    public void OnRightStick(InputAction.CallbackContext value)
    {
        leadTargetLocation = value.ReadValue<Vector2>() * lead;
    }

    */

    private Rect cameraBounds = new Rect();
    private void Update()
    {
        var target = PlayerController.Instance.transform;

        leadLocation = Vector2.Lerp(leadLocation, leadTargetLocation, transition * Time.deltaTime);
        var leadPos = target.position + (Vector3)leadLocation;
        transform.position = new Vector3(
            Mathf.Clamp(leadPos.x, -MaxBounds.x, MaxBounds.x), 
            Mathf.Clamp(leadPos.y, -MaxBounds.y, MaxBounds.y),
            transform.position.z);

        var zoom = ForceMaxZoom ? (new Vector2(MaxBounds.x,MaxBounds.y)).magnitude * 2f : ((Vector2)transform.position - center).magnitude;

        camera.orthographicSize = Mathf.Lerp(camera.orthographicSize,
            Mathf.Clamp(zoom, minZoom, maxZoom), 
            Time.unscaledDeltaTime);

        Background.localScale = Vector3.one * 0.25f * camera.orthographicSize;

        cameraBounds = BuildRect(transform.position, new Vector2(camera.orthographicSize * 1.25f * camera.aspect, camera.orthographicSize * 2));

        if (cameraBounds.Contains((Vector2)target.position - cameraBounds.position, true))
        {
            positionIndicator.gameObject.SetActive(false);
        }
        else
        {
            positionIndicator.gameObject.SetActive(true);
            positionIndicator.Angle = Vector2.SignedAngle(Vector2.right, target.position - transform.position);
        }
    }

    private Rect BuildRect(Vector2 position, Vector2 bounds)
    {
        return new Rect((position - (bounds / 2f))/2f, bounds);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;

        var bottomLeft = cameraBounds.min + cameraBounds.position;
        var bottomRight = new Vector2(cameraBounds.max.x, cameraBounds.min.y) + cameraBounds.position;
        var topRight = cameraBounds.max + cameraBounds.position;
        var topLeft = new Vector2(cameraBounds.min.x, cameraBounds.max.y) + cameraBounds.position;

        Gizmos.DrawLine(topLeft, topRight);
        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomRight, bottomLeft);
        Gizmos.DrawLine(bottomLeft, topLeft);
    }
}
