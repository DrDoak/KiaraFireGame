using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using UnityEngine;

public class PlayerCharacter : Character
{
    public static PlayerCharacter Instance;
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
    private float horizontalMovement;
    private bool hasMidAirJump = false;
    [SerializeField]
    private float boostedMaxSpeed = 20;
    [SerializeField]
    private int hitsToReachMaxBoost = 4;
    [SerializeField]
    private float boostDecayTime = 1;
    [SerializeField]
    private GameObject detectionBoxUp;
    [SerializeField]
    private GameObject detectionBoxSide;
    [SerializeField]
    private GameObject detectionBoxDown;

    private float lastTimeHeldInDirection;
    private int lastDirectionBoost;

    private void Awake()
    {
        components = GetComponent<CharacterComponents>();
        sensors = GetComponent<SurroundingSensors>();
        Instance = this;
        baseMaxSpeed = maxSpeed;
    }
    private void Start()
    {
        components.MAttackConfirm.attackConfirm += OnAttackConfirm;
    }
    private void OnDestroy()
    {
        components.MAttackConfirm.attackConfirm -= OnAttackConfirm;
    }
    public void OnAttackConfirm(Hitbox hb, Hurtbox hurt, Attackable objectHit)
    {
        if (hb.damage > 0)
        {
            ReplenishMidAirJump();
        }
        if (hb.propelonHitConfirm.magnitude != 0)
        {
            if (hb.propelonHitConfirm.y > 0 && components.MMovement.Velocity.y < 0)
            {
                components.MMovement.ResetVerticalVelocity();
            }
            Vector2 propel = new Vector2((components.MMovement.FacingLeft ? -1 : 1) * hb.propelonHitConfirm.x, hb.propelonHitConfirm.y);
            components.MMovement.ApplyImpulse(propel);
        }
        if (lastDirectionBoost == 0 && Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.A))
            {
                lastDirectionBoost = -1;
            }  else if (Input.GetKey(KeyCode.D))
            {
                lastDirectionBoost = 1;
            }
        } 
        if (lastDirectionBoost == -1 && Input.GetKey(KeyCode.A))
        {
            maxSpeed += (boostedMaxSpeed - baseMaxSpeed) / hitsToReachMaxBoost;
        } else if (lastDirectionBoost == 1 && Input.GetKey(KeyCode.D))
        {
            maxSpeed += (boostedMaxSpeed - baseMaxSpeed) / hitsToReachMaxBoost;
        }
        maxSpeed = Mathf.Clamp(maxSpeed, baseMaxSpeed, boostedMaxSpeed);
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
        if (components.MMovement.Grounded())
        {
            ReplenishMidAirJump();
        }
        UpdateTimers();
        ProcessState();
        if (currentHitStun == 0 && !components.MAnimatorOptions.InAction)
        {
            AnimateNeutral();
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
    }

    private void ProcessState()
    {
        if (currentHitStun > 0)
        {
            HitStunState();
        } 
        if (components.MAnimatorOptions.hasControl)
        {
            ProcessMaxSpeedAdjustments();
            ProcessMovement();
        } else
        {
            canBlock = true;
        }
    }
    void ProcessMaxSpeedAdjustments()
    {
        if (maxSpeed == baseMaxSpeed) return;
        if (lastDirectionBoost == -1 && Input.GetKey(KeyCode.A) ||
            lastDirectionBoost == 1 && Input.GetKey(KeyCode.D))
        {
            lastTimeHeldInDirection = components.MScalableTime.TimeSinceLevelLoad();
        } else
        {
            if (components.MScalableTime.TimeSinceLevelLoad() - lastTimeHeldInDirection > boostDecayTime)
            {
                maxSpeed = baseMaxSpeed;
                lastDirectionBoost = 0;
            }
        }
    }
    void HitStunState()
    {
        
        DecreaseHitStun();
    }
    void ProcessMovement()
    {
        horizontalMovement = 0;
        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)) horizontalMovement--;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.UpArrow)) horizontalMovement++;
        AttemptHorizontalMovement(horizontalMovement);
        HandleJump(Input.GetKey(KeyCode.Space), Input.GetKeyDown(KeyCode.Space));
        canBlock = !Input.GetKey(KeyCode.J);
        UpdateHoldAttack(!canBlock);
    }
    void UpdateHoldAttack(bool isHoldAttack)
    {
        detectionBoxUp.SetActive(false);
        detectionBoxSide.SetActive(false);
        detectionBoxDown.SetActive(false);
        if (!isHoldAttack)
        {
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (HasSmallCharge()) components.MAnimatorOptions.PerformActionAnimation("fire_up");
            detectionBoxUp.SetActive(true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (HasSmallCharge()) components.MAnimatorOptions.PerformActionAnimation("fire_down");
            detectionBoxDown.SetActive(true);
        }
        else
        {
            detectionBoxSide.SetActive(true);
            if (HasSmallCharge()) components.MAnimatorOptions.PerformActionAnimation("fire");
        }
        lastTimeHeldAttack = components.MScalableTime.TimeSinceLevelLoad();
    }
    void AttemptJump()
    {

    }

    private void HandleJump(bool jumpPressed, bool jumpPressedFrame)
    {
        // Start the initial jump
        if (jumpPressed && _coyoteTimeCounter > 0 )
        {
            _coyoteTimeCounter = 0;
            _jumpTimeCounter = jumpTime;
        }
        if (jumpPressedFrame && !sensors.Grounded && hasMidAirJump)
        {
            _jumpTimeCounter = jumpTime;
            components.MMovement.ResetVerticalVelocity();
            hasMidAirJump = false;
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
    public void ReplenishMidAirJump()
    {
        hasMidAirJump = true;
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
        components.mAnimator.speed = 1;
        if (sensors.Grounded)
        {
           Vector2 currentSpeed = components.MMovement.Velocity;
            if (Mathf.Abs(currentSpeed.x) > 0.5f)
            {
                if (horizontalMovement != 0)
                {
                    components.mAnimator.speed = (Mathf.Abs(currentSpeed.x) / baseMaxSpeed);
                    components.mAnimator.Play("run");
                } else
                {
                    components.mAnimator.Play("stop");
                }
                
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
