using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCharacter : Character
{
    [SerializeField]
    private float jumpForce;
    [SerializeField]
    private float addedJumpForce;
    [SerializeField]
    private float jumpTime;
    [SerializeField]
    private float CoyoteTime;
    [Header("Charge Attacks")]
    public float chargeTimeSmall = 0.3f;
    public float chargeTimeLarge = 1.5f;

    private float _jumpTimeCounter;
    private float _coyoteTimeCounter;
    private SurroundingSensors sensors;
    private float lastTimeHeldAttack;
    private void Awake()
    {
        components = GetComponent<CharacterComponents>();
        sensors = GetComponent<SurroundingSensors>();
    }
    public bool HasSmallCharge()
    {
        return (components.MScalableTime.TimeSinceLevelLoad() - lastTimeHeldAttack) > chargeTimeSmall;
    }
    public bool HasLargeCharge()
    {
        return (components.MScalableTime.TimeSinceLevelLoad() - lastTimeHeldAttack) > chargeTimeLarge;
    }
    // Update is called once per frame
    void Update()
    {
        UpdateTimers();
        ProcessState();
        if (!components.MAnimatorOptions.InAction)
        {
            AnimateNeutral();
        }
    }

    private void ProcessState()
    {
        if (currentHitStun > 0)
        {
            HitStunState();
            return;
        } 
        if (components.MAnimatorOptions.hasControl)
        {
            ProcessMovement();
        }
    }

    void HitStunState()
    {
        DecreaseHitStun();
    }
    void ProcessMovement()
    {
        float horizontalMovement = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontalMovement--;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.UpArrow)) horizontalMovement++;
        AttemptHorizontalMovement(horizontalMovement);
        HandleJump(Input.GetKey(KeyCode.Space));
        if (Input.GetKey(KeyCode.J))
        {
            HoldAttack();
        }
    }
    void HoldAttack()
    {
        if (Input.GetKey(KeyCode.W))
        {
            components.MAnimatorOptions.PerformActionAnimation("fire_up");
        }
        else if (Input.GetKey(KeyCode.S))
        {
            components.MAnimatorOptions.PerformActionAnimation("fire_down");
        }
        else
        {
            components.MAnimatorOptions.PerformActionAnimation("fire");
        }
        lastTimeHeldAttack = components.MScalableTime.TimeSinceLevelLoad();
    }
    void AttemptJump()
    {

    }

    private void HandleJump(bool jumpPressed)
    {
        // Start the initial jump
        if (jumpPressed && _coyoteTimeCounter > 0)
        {
            float jumpVelocity = jumpForce;
            _coyoteTimeCounter = 0;

            _jumpTimeCounter = jumpTime;
        }

        // check if the ceiling was hit
        if (!sensors.HitCeiling)
            Jump(jumpPressed, jumpForce, addedJumpForce, jumpTime);
        else
            _jumpTimeCounter = 0;

        // stop jumping if released earlier
        if (!jumpPressed  && _jumpTimeCounter > 0 && !sensors.Grounded)
        {
            _jumpTimeCounter = 0;
        }

        //// apply a downwards force to fall faster
        //if (_jumpTimeCounter <= 0)
        //{
        //    _body.linearVelocityY += airborneData.GravityMultiplier * Physics2D.gravity.y * Time.deltaTime;

        //    // Clamp the falling velocity to avoid falling too fast
        //    _body.linearVelocityY = Mathf.Clamp(_body.linearVelocityY, -airborneData.MaxFallingSpeed, Mathf.Infinity);
        //}
    }

    private void Jump(bool jumpPressed, float jumpForce, float addedJumpForce, float jumpTime)
    {
        // When jump is being held, jump higher
        if (jumpPressed && _jumpTimeCounter > 0)
        {
            components.MMovement.SetPersonalVelocityY( Mathf.Lerp(addedJumpForce, jumpForce, _jumpTimeCounter / jumpTime));
            _jumpTimeCounter -= components.MScalableTime.DeltaTime;
        }
    }


    public void UpdateTimers()
    {
        if (sensors.Grounded)
        {
            _coyoteTimeCounter = CoyoteTime;
            _jumpTimeCounter = jumpTime;
        }
        else
            _coyoteTimeCounter -= Time.deltaTime;
    }

    public void AnimateNeutral()
    {
        if (sensors.Grounded)
        {
            Vector2 currentSpeed = components.MMovement.Velocity;
            if (Mathf.Abs(currentSpeed.x) > 0.5f)
            {
                components.mAnimator.Play("run");
            } else
            {
                components.mAnimator.Play("idle");
            }
        } else
        {
            Vector2 currentSpeed = components.MMovement.Velocity;
            if (currentSpeed.y > 0.0f)
            {
                components.mAnimator.Play("jump");
            }
            else
            {
                components.mAnimator.Play("fall");
            }
        }
    }

}
