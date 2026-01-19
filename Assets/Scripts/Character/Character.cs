using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character : MonoBehaviour
{
    [SerializeField]
    private float acceleration;
    [SerializeField]
    protected float maxSpeed;
    protected float currentHitStun = 0;
    protected CharacterComponents components;
    public delegate void StateChangedEvent();
    public StateChangedEvent EnterHitStun;
    public StateChangedEvent ExitHitStun;
    public bool canBlock;
    // Start is called before the first frame update
    protected float baseMaxSpeed;
    private void Awake()
    {
        components = GetComponent<CharacterComponents>();
        baseMaxSpeed = maxSpeed;
    }
    // Update is called once per frame
    void Update()
    {
        DecreaseHitStun();
    }
    public void ApplyHitStun(float hitstun)
    {
        if (currentHitStun == 0)
        {
            if (EnterHitStun != null)
            {
                EnterHitStun();
            }
        }
        currentHitStun = hitstun;
        if (canBlock)
        {
            currentHitStun = hitstun * 0.5f;
            components.MAnimatorOptions.PlayAnimation("block");
        } else
        {
            currentHitStun = hitstun;
            components.MAnimatorOptions.PlayAnimation("hurt");
        }
        
    }

    protected void DecreaseHitStun()
    {
        if (currentHitStun <= 0) return;
        currentHitStun -= components.MScalableTime.DeltaTime;
        if (currentHitStun <= 0)
        {
            currentHitStun = 0;
            if (ExitHitStun != null)
            {
                ExitHitStun();
            }
        }
    }

    protected void AttemptHorizontalMovement(float movementInput)
    {
        // return if no input is given
        if (!(Mathf.Abs(movementInput) > 0)) return;

        // increment velocity by acceleration
        float increment = movementInput * acceleration;
        float newSpeed = Mathf.Clamp(components.MMovement.PersonalVelocity.x + increment, -maxSpeed, maxSpeed);
        components.MMovement.SetPersonalVelocityX(newSpeed);
        // flip object based on direction
        float direction = Mathf.Sign(movementInput);
        components.MMovement.SetFacingLeft(direction < 0);
    }
}
