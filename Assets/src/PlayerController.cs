using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    private Vector2 currentMove;
    private Vector2 currentFacingDir = Vector2.up;

    [SerializeField]
    private float rotationPerSecond = 180f;

    [SerializeField]
    private float thrusterForce = 100f;

    [SerializeField]
    private float max_velocity = 20f;

    private new Rigidbody2D rigidbody2D;

    [SerializeField]
    private Transform modelTransform;
    [SerializeField]
    private Transform fireTransform;

    private Vector3 basicmodeltransform;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();

        basicmodeltransform = modelTransform.localScale;
    }

    public void OnRightStick(InputAction.CallbackContext value)
    {
        currentMove = value.ReadValue<Vector2>();
        fireTransform.localScale = new Vector3(1f, currentMove.magnitude, 1f);
        modelTransform.localScale = basicmodeltransform + new Vector3(0f, -basicmodeltransform.y, 0f) * Mathf.Clamp(currentMove.magnitude, 0f, 0.1f);
    }

    public void Pickup(Pickup toPickup)
    {
        toPickup.PickupAction(this);
    }

    private void Update()
    {
        if(currentMove != Vector2.zero)
        {
            var angle = Vector2.SignedAngle(currentFacingDir, currentMove);

            if(Mathf.Abs(angle) < 0.1f)
            {
                currentFacingDir = currentMove;
            }
            if(Mathf.Abs(angle) > 0.1f)
            {
                currentFacingDir = (Quaternion.Euler(0, 0, angle * rotationPerSecond * Time.deltaTime) * currentFacingDir).normalized;
            }
        }
    }

    private void FixedUpdate()
    {
        rigidbody2D.AddForce(currentMove * thrusterForce, ForceMode2D.Force);

        rigidbody2D.SetRotation(Vector2.SignedAngle(Vector2.up, currentFacingDir));

        if(rigidbody2D.velocity.magnitude > max_velocity)
        {
            Debug.Log("Hitting Max V");
            rigidbody2D.velocity = rigidbody2D.velocity.normalized * max_velocity;
        }
    }
}
