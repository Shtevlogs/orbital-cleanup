using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alien : MonoBehaviour
{
    public float MaxAimRange = 10f;
    public float MinAimRange = 3f;
    public float SuccRange = 4f;
    public float CaptureRange = 1f;

    public float RotationTorque = 5f;
    public float SuccForce = 1f;

    public float ExpulsionForce = 2f;

    public Animator SuccAnimator;
    public Animator CountdownAnimator;

    public AudioReference SuccSFX;
    public AudioReference ShootSFX;

    public Transform AimIndicator;

    private Rigidbody2D body;

    public float AimTimeTotal = 4f;
    private float aimTimeLeft = 0f;

    private float coolDown = 0f;

    private bool aiming = false;

    private enum AlienState
    {
        Nothing,
        Searching,
        Succing,
        Aiming
    }

    private AlienState myState;

    private void Start()
    {
        body = GetComponent<Rigidbody2D>();

        SuccSFX.Init();
        ShootSFX.Init();
    }

    private void _updateAnimators()
    {
        if(myState == AlienState.Nothing)
        {
            SuccSFX.Toggle(false);
            SuccAnimator.SetBool("Succ", false);
            CountdownAnimator.SetBool("Countdown", false);
            CountdownAnimator.SetFloat("Rate", 0f);
        }
        else if(myState == AlienState.Searching)
        {
            SuccSFX.Toggle(false);
            SuccAnimator.SetBool("Succ", false);
            CountdownAnimator.SetBool("Countdown", false);
            CountdownAnimator.SetFloat("Rate", 0f);
        }
        else if(myState == AlienState.Succing)
        {
            SuccSFX.Toggle(true);
            SuccAnimator.SetBool("Succ", true);
            CountdownAnimator.SetBool("Countdown", false);
            CountdownAnimator.SetFloat("Rate", 0f);
        }
        else if(myState == AlienState.Aiming)
        {
            SuccSFX.Toggle(false);
            SuccAnimator.SetBool("Succ", false);
            CountdownAnimator.SetBool("Countdown", true);
            CountdownAnimator.SetFloat("Rate", 1f - (aimTimeLeft / AimTimeTotal));
        }
    }

    private void Update()
    {
        //aquire target
        var target = PlayerController.Instance;
        var targetDist = target == null ? float.MaxValue : (target.transform.position - transform.position).magnitude;

        coolDown -= Time.deltaTime;

        if (coolDown <= 0f && targetDist <= CaptureRange)
        {
            myState = AlienState.Aiming;
        }
        else if (coolDown <= 0f && targetDist <= SuccRange) 
        {
            myState = AlienState.Succing;
        }
        else if(coolDown <= 0f && targetDist <= MaxAimRange)
        {
            myState = AlienState.Searching;
        }
        else
        {
            myState = AlienState.Nothing;
        }

        _updateAnimators();
        _updateLogic();
    }

    private void _updateLogic()
    {
        if (myState == AlienState.Nothing)
        {
            //donothing
            body.freezeRotation = false;
        }
        else if(myState == AlienState.Searching)
        {
            _turnToPlayer();
        }
        else if(myState == AlienState.Succing)
        {
            _turnToPlayer();
            _acceleratePlayer();
        }
        else if(myState == AlienState.Aiming)
        {
            if (!aiming)
            {
                //first time setup
                aimTimeLeft = AimTimeTotal;
                aiming = true;
                PlayerController.Instance.Captured(true);
            }

            _turnWithPlayer();
            _holdPlayer();
            _shootPlayer();
        }
    }

    private bool shotPrimed = false;

    private void _holdPlayer()
    {
        if (PlayerController.Instance == null) return;

        var playerBody = PlayerController.Instance.GetComponent<Rigidbody2D>();

        playerBody.transform.position = transform.position;
    }

    private void _shootPlayer()
    {
        if (PlayerController.Instance == null) return;

        aimTimeLeft -= Time.deltaTime;
        if (aimTimeLeft <= 0f)
        {
            _shootLogic();
            return;
        }

        if (PlayerController.Instance.HasZeroMovement())
        {
            if (shotPrimed)
            {
                _shootLogic();
            }
        }
        else
        {
            shotPrimed = true;
        }
    }

    private void _shootLogic()
    {
        var shotDir = (Vector2)(PlayerController.Instance.transform.rotation * Vector2.up);

        PlayerController.Instance.Captured(false);

        var playerBody = PlayerController.Instance.GetComponent<Rigidbody2D>();
        playerBody.velocity = GetComponent<Rigidbody2D>().velocity + shotDir * ExpulsionForce;

        coolDown = 0.5f;
        shotPrimed = false;
        aiming = false;

        ShootSFX.Play();
    }

    private void _turnWithPlayer()
    {
        if (PlayerController.Instance == null) return;

        body.freezeRotation = true;

        body.SetRotation(180f + PlayerController.Instance.transform.rotation.eulerAngles.z);
    }

    private void _turnToPlayer()
    {
        var targetingDeets = new TargetingDetails(PlayerController.Instance, transform);

        body.freezeRotation = true;

        body.rotation = Vector2.SignedAngle(Vector2.down, targetingDeets.Direction);
    }

    private void _acceleratePlayer()
    {
        var targetingDeets = new TargetingDetails(PlayerController.Instance, transform);
        var playerBody = PlayerController.Instance?.GetComponent<Rigidbody2D>();

        if(playerBody != null)
        {
            var x = targetingDeets.X(CaptureRange, SuccRange);
            playerBody.AddForce(-targetingDeets.Direction * x * x * x * SuccForce, ForceMode2D.Force);
        }
    }

    private class TargetingDetails
    {
        public Vector2 Direction;
        public float Magnitude;

        public TargetingDetails(PlayerController target, Transform me)
        {
            if(target == null) { return; }

            var line = (Vector2)(target.transform.position - me.position);
            Direction = line.normalized;
            Magnitude = line.magnitude;
        }

        public float X(float min, float max)
        {
            return Mathf.Clamp01(1f - ((Magnitude - min) / (max - min)));
        }
    }
}
