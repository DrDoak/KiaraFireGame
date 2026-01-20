using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class BaseMovement : MonoBehaviour
{
    public bool FacingLeft {  get { return facingLeft; } }
    public Vector2 Velocity {  get { return trueVelocity / Time.fixedDeltaTime; } }
    public Vector2 PersonalVelocity { get { return selfVelocity; } }
    private bool facingLeft;
    [SerializeField]
    private Transform animatedObjsTransform;

    private Vector2 trueVelocity = new Vector2();
    public CharacterComponents components;
    private Rigidbody2D mRigidBody;
    private List<FixedMovementVector> fixedMovements = new List<FixedMovementVector>();
    private List<FixedMovementVector> toRemove = new List<FixedMovementVector>();
    private Vector2 selfVelocity = new Vector2();
    [SerializeField]
    private float horizontalDeceleration;
    [SerializeField]
    private float horizontalDecelerationAir;
    [SerializeField]
    private float terminalVelocity;
    [SerializeField]
    private float gravity;

    private Vector3 lastPosition = new Vector3();
    public bool applyYDecel;
    private SurroundingSensors sensors;
    public void SetFacingLeft(bool newFacingLeft)
    {
        if (facingLeft == newFacingLeft) return;

        facingLeft = newFacingLeft;
        animatedObjsTransform.localScale = new Vector3(-animatedObjsTransform.localScale.x, animatedObjsTransform.localScale.y, animatedObjsTransform.localScale.z);
    }
    // Start is called before the first frame update
    void Awake()
    {
        components = GetComponent<CharacterComponents>();
        mRigidBody = GetComponent<Rigidbody2D>();
        sensors = GetComponent<SurroundingSensors>();
    }

    // Update is called once per frame
    void Update()
    {
        foreach(FixedMovementVector v in fixedMovements)
        {
            if (v.IsExpired()) { toRemove.Add(v); }
        }
        toRemove.ForEach(v => fixedMovements.Remove(v));
        toRemove.Clear();
        mRigidBody.isKinematic = components.MScalableTime.TimeScale() == 0;
    }
    public bool Grounded()
    {
        return sensors.Grounded;
    }
    public void SetPersonalVelocity(Vector2 personalVelocity)
    {
        selfVelocity = personalVelocity;
    }
    public void SetPersonalVelocityX(float newX)
    {
        selfVelocity = new Vector2(newX, selfVelocity.y);
    }
    public void SetPersonalVelocityY(float newY)
    {
        selfVelocity = new Vector2(selfVelocity.x, newY);
    }
    public void ResetVerticalVelocity()
    {
        selfVelocity = new Vector2(selfVelocity.x, 0);
    }
    public void ApplyImpulse(Vector2 impulse)
    {
        selfVelocity += impulse;
    }
    public void AddFixedMovements(Vector2 vector, float duration)
    {
        fixedMovements.Add(new FixedMovementVector(this, vector, duration));
    }
    private void FixedUpdate()
    {
        Vector2 velocity = new Vector2();
        foreach(FixedMovementVector v in fixedMovements)
        {
            velocity += (v.direction);
        }
        if (Mathf.Abs(selfVelocity.x) > 0.05f)
        {
            selfVelocity.x -= (horizontalDeceleration * selfVelocity.x * components.MScalableTime.FixedDeltaTime);
        } else
        {
            selfVelocity.x = 0;
        }
        if (applyYDecel)
        {
            if (Mathf.Abs(selfVelocity.y) > 0.05f)
            {
                selfVelocity.y -= (horizontalDeceleration * selfVelocity.y * components.MScalableTime.FixedDeltaTime);
            }
            else
            {
                selfVelocity.y = 0;
            }
        }
        
        if (gravity != 0)
        {
            if (sensors.Grounded && selfVelocity.y < 0)
            {
                selfVelocity.y = 0;
            }
            if (selfVelocity.y > terminalVelocity && !sensors.Grounded)
            {
                selfVelocity.y -= (gravity * components.MScalableTime.FixedDeltaTime);
            }
        }
       
        velocity += selfVelocity;
        mRigidBody.velocity = velocity;
        trueVelocity = (transform.position - lastPosition);
        lastPosition = transform.position;
    }
}

public class FixedMovementVector
{
    public Vector2 direction;
    public float expirationTime;
    private ScalableTime time;

    public FixedMovementVector(BaseMovement obj, Vector2 direction, float duration)
    {
        this.direction = direction;
        time = obj.components.MScalableTime;
        expirationTime = time.TimeSinceLevelLoad() + duration;
    }
    public bool IsExpired()
    {
        return time.TimeSinceLevelLoad() > expirationTime;
    }
}