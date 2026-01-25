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

    [SerializeField]
    private ParticleSystem midairavaliable;
    [SerializeField]
    private ParticleSystem midairRegained;

    [SerializeField]
    private ParticleSystem speedlvl1;
    [SerializeField]
    private ParticleSystem speedlvl2;
    [SerializeField]
    private TrailRenderer speedlvl3;

    [SerializeField]
    private string jumpSFX;
    [SerializeField]
    private string doublejumpSFX;
    [SerializeField]
    private GameObject doubleJumpVFX;

    private float lastTimeHeldInDirection;
    private int lastDirectionBoost;

    //Boost
    private const float boost_delay = 0.05f;
    private float last_boost_applied_time;
    public float FireCooldown { get { return fireCooldown; } }
    private float fireCooldown;

    private bool useSecondSlash;
    [SerializeField]
    private bool burn;
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
        ApplyBoost(hb);
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
    private void ApplyBoost(Hitbox hb)
    {
        if (hb.propelonHitConfirm.magnitude == 0)
        {
            return;
        }
        if (components.MScalableTime.TimeSinceLevelLoad() - last_boost_applied_time < boost_delay)
        {
            return;
        }
        last_boost_applied_time = components.MScalableTime.TimeSinceLevelLoad();
        if (hb.propelonHitConfirm.magnitude != 0)
        {
            if (hb.propelonHitConfirm.y != 0)
            {
                components.MMovement.ResetVerticalVelocity();
            }
            Vector2 propel = new Vector2((components.MMovement.FacingLeft ? -1 : 1) * hb.propelonHitConfirm.x, hb.propelonHitConfirm.y);
            components.MMovement.ApplyImpulse(propel);
        }
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
        fireCooldown = Mathf.Max(0, fireCooldown - components.MScalableTime.DeltaTime);
        if (components.MMovement.Grounded())
        {
            ReplenishMidAirJump();
        }
        if (burn)
        {
            components.MAttackable.ChangeHP(-components.MScalableTime.DeltaTime);
        }
        UpdateTimers();
        ProcessState();
        UpdateChargeParticles();
        UpdateMaxSpeed();
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
        if (!attemptingDive())
        {
            HandleJump(Input.GetKey(KeyCode.Space), Input.GetKeyDown(KeyCode.Space));
        }
        canBlock = !Input.GetKey(KeyCode.J);
        UpdateAttacks(!canBlock);
    }
    void UpdateAttacks(bool isHoldAttack)
    {
        if (!sensors.Grounded && attemptingDive())
        {
            components.MAnimatorOptions.PerformActionAnimation("dive");
        }
        detectionBoxUp.SetActive(false);
        detectionBoxSide.SetActive(false);
        detectionBoxDown.SetActive(false);
        if (!isHoldAttack)
        {
            return;
        }
        if (Input.GetKey(KeyCode.W))
        {
            if (HasSmallCharge())
            {
                components.MAnimatorOptions.PerformActionAnimation("fire_up");
                fireCooldown = 0.12f;
            }
            detectionBoxUp.SetActive(true);
        }
        else if (Input.GetKey(KeyCode.S))
        {
            if (HasSmallCharge())
            {
                components.MAnimatorOptions.PerformActionAnimation("fire_down");
                fireCooldown = 0.12f;
            }
            detectionBoxDown.SetActive(true);
        }
        else
        {
            detectionBoxSide.SetActive(true);
            if (HasSmallCharge())
            {
                components.MAnimatorOptions.PerformActionAnimation("fire");
                fireCooldown = 0.12f;
            }
        }
        lastTimeHeldAttack = components.MScalableTime.TimeSinceLevelLoad();
        
    }
    void AttemptJump()
    {

    }
    private bool attemptingDive()
    {
        return Input.GetKey(KeyCode.S) && Input.GetKey(KeyCode.Space) && !Input.GetKey(KeyCode.J);
    }
    private void HandleJump(bool jumpPressed, bool jumpPressedFrame)
    {
        // Start the initial jump
        if (jumpPressed && _coyoteTimeCounter > 0 )
        {
            _coyoteTimeCounter = 0;
            _jumpTimeCounter = jumpTime;
            
        }
        if (jumpPressedFrame && sensors.Grounded && jumpSFX != "") { 
            components.mAudio.PlaySounds(jumpSFX);
        }
        if (jumpPressedFrame && !sensors.Grounded && hasMidAirJump)
        {
            _jumpTimeCounter = jumpTime;
            components.MMovement.ResetVerticalVelocity();
            hasMidAirJump = false;
            if (doublejumpSFX != "")
            {
                components.mAudio.PlaySounds(doublejumpSFX);
            }
            if (doubleJumpVFX != null)
            {
                components.mSpawner.SpawnObject(doubleJumpVFX);
            }
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

    public void AlternateSlash()
    {
        if (sensors.Grounded)
        {
            components.MAnimatorOptions.PerformActionAnimation(useSecondSlash ? "atk_air2" : "atk_air");
        } else
        {
            components.MAnimatorOptions.PerformActionAnimation(useSecondSlash ? "atk_ground2" : "atk_ground");
        }
        useSecondSlash = !useSecondSlash;
    }
    private void AnimateNeutral()
    {
        string suffix = "";
        if (!Input.GetKey(KeyCode.J))
        {
            suffix = "_neutral";
        }
        components.mAnimator.speed = 1;
        if (sensors.Grounded)
        {
           Vector2 currentSpeed = components.MMovement.Velocity;
            if (Mathf.Abs(currentSpeed.x) > 0.5f)
            {
                if (horizontalMovement != 0)
                {
                    components.mAnimator.speed = (Mathf.Abs(currentSpeed.x) / baseMaxSpeed);
                    if (currentSpeed.magnitude >= boostedMaxSpeed * 0.9f)
                    {
                        components.mAnimator.Play("sprint" + suffix);
                    } else
                    {
                        components.mAnimator.Play("run" + suffix);
                    }
                } else
                {
                    components.mAnimator.Play("stop" + suffix);
                }
                
            } else
            {
                components.mAnimator.Play("idle" + suffix);
            }
        } else
        {
            Vector2 currentSpeed = components.MMovement.Velocity;
            if (currentSpeed.y > 0.0f)
            {
                components.mAnimator.Play("jump" + suffix);
            }
            else
            {
                components.mAnimator.Play("fall" + suffix);
            }
        }
    }

    private void UpdateChargeParticles()
    {
        if (HasSmallCharge() && !midairavaliable.isPlaying)
        {
            midairavaliable.Play();
            midairRegained.Stop();
            midairRegained.Play();
            components.mAudio.PlaySounds("Have_charge");
        }
        if (!HasSmallCharge() && midairavaliable.isPlaying)
        {
            midairavaliable.Stop();
        }
    }

    private void UpdateMaxSpeed()
    {
        Vector2 currentSpeed = components.MMovement.Velocity;
        if (currentSpeed.magnitude < (baseMaxSpeed * 0.8f) || maxSpeed == baseMaxSpeed)
        {
            speedlvl1.Stop();
            speedlvl2.Stop();
            speedlvl3.emitting = false;
            return;
        }
        float diff = (boostedMaxSpeed - baseMaxSpeed);
        if (maxSpeed >= (baseMaxSpeed + (diff / hitsToReachMaxBoost)))
        {
            speedlvl1.Play();
        }
        if (maxSpeed >= (baseMaxSpeed + ((2* diff) / hitsToReachMaxBoost)))
        {
            speedlvl2.Play();
        }
        if (maxSpeed >= (baseMaxSpeed + ((3 * diff) / hitsToReachMaxBoost)))
        {
            speedlvl3.emitting = true;
        }
    }

}
