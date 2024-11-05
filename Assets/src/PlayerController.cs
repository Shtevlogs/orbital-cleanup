using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController Instance;

    private void Awake()
    {
        Instance = this;
    }

    [NonSerialized]
    private Vector2 currentMove;
    private Vector2 currentFacingDir = Vector2.up;

    public bool infiniteFuel = false;

    [SerializeField]
    private float rotationPerSecond = 180f;

    [SerializeField]
    private float thrusterForce = 100f;

    [SerializeField]
    private float max_velocity = 20f;

    [SerializeField]
    private float fuelDrainRate = 0.2f;

    private new Rigidbody2D rigidbody2D;

    [SerializeField]
    private Transform modelTransform;
    [SerializeField]
    private Transform fireTransform;

    private Vector3 basicmodeltransform;

    public float FuelLevel = 1f;
    public int MaxHealth = 3;
    [NonSerialized]
    public int Health = 3;
    [NonSerialized]
    public int ScrapCollectedCount = 0;

    [SerializeField]
    private AudioReference thrusterAudio;

    [SerializeField]
    private AudioReference pickupAudio;
    [SerializeField]
    private AudioClip fuelPickupClip;
    [SerializeField]
    private AudioClip scrapPickupClip;

    private bool captured;

    private OrbitalRenderer orbitalRender;

    [SerializeField]
    private Animator MyAnimator;

    public static float FullMusicRadius = 1f;
    public static float NoMusicRadius = 10f;

    private void Start()
    {
        rigidbody2D = GetComponent<Rigidbody2D>();
        orbitalRender = GetComponentInChildren<OrbitalRenderer>();

        basicmodeltransform = modelTransform.localScale;

        thrusterAudio.Init();
        pickupAudio.Init();
    }

    public void OnRightStick(InputAction.CallbackContext value)
    {
        currentMove = value.ReadValue<Vector2>();

        //deadzone
        if(currentMove.magnitude < 0.25f)
        {
            currentMove = Vector2.zero;
        }

        fireTransform.localScale = new Vector3(1f, FuelLevel > 0f ? currentMove.magnitude: 0f, 1f);
        modelTransform.localScale = basicmodeltransform - new Vector3(0f, basicmodeltransform.y, 0f) * Mathf.Clamp(FuelLevel > 0f ? currentMove.magnitude : 0f, 0f, 0.1f);
    }

    public void Damage()
    {
        Health -= 1;

        if(Health <= 0)
        {
            GameStateManager.EndRound(false, "Mission Failed!");
        }
    }

    public void Pickup(Pickup toPickup)
    {
        toPickup.PickupAction(this);

        MyAnimator.SetTrigger("Pickup");

        if (toPickup is Scrap)
        {
            pickupAudio.Play(scrapPickupClip);
        }
        else if(toPickup is Fuel)
        {
            pickupAudio.Play(fuelPickupClip);
        }
    }

    public void Captured(bool captured)
    {
        this.captured = captured;

        if (captured)
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeAll;
            orbitalRender.gameObject.SetActive(false);
        }
        else
        {
            rigidbody2D.constraints = RigidbodyConstraints2D.FreezeRotation;
            orbitalRender.gameObject.SetActive(true);
        }

        CameraBehaviour.Instance.ForceMaxZoom = captured;
    }

    public bool HasZeroMovement()
    {
        return currentMove == Vector2.zero;
    }

    private void Update()
    {
        var dist = transform.position.magnitude;
        MusicController.UpdatePlanetDistance(dist < FullMusicRadius ? 0f : ((dist - FullMusicRadius) / NoMusicRadius));

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
        if(FuelLevel > 0f && !captured)
        {
            var fuelDrain = currentMove.magnitude * fuelDrainRate * Time.fixedDeltaTime;

            thrusterAudio.Fade(currentMove.magnitude);

            rigidbody2D.AddForce(currentMove * thrusterForce, ForceMode2D.Force);

            FuelLevel -= infiniteFuel ? 0f : fuelDrain;
        }
        else
        {
            thrusterAudio.Fade(0f);
        }

        rigidbody2D.SetRotation(Vector2.SignedAngle(Vector2.up, currentFacingDir));

        if(rigidbody2D.velocity.magnitude > max_velocity)
        {
            Debug.Log("Hitting Max V");
            rigidbody2D.velocity = rigidbody2D.velocity.normalized * max_velocity;
        }
    }
}
