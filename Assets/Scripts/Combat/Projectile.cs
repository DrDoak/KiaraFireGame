using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    CharacterComponents components;
    [SerializeField]
    private float lifeTime = 5f;
    [SerializeField]
    private int maxHits = 1;
    [SerializeField]
    public Vector2 initialVelocity;
    [SerializeField]
    private bool targetPlayer;
    [SerializeField]
    private float setAngleOffset = 0;
    [SerializeField]
    private string hitSFX;

    private int numHits;
    private float expirationTime;
    public AttackConfirm parentAttackConfirm;
    private bool initialized;

    void InitializeProjectile()
    {
        components = GetComponent<CharacterComponents>();
        expirationTime = components.MScalableTime.TimeSinceLevelLoad() + lifeTime;
        initialized = true;
        if (initialVelocity != null)
        {
            if (targetPlayer)
            {
                TargetPlayer();
            } else
            {
                InitializeSetVelocity();
            }
        }
    }
    public void SetAngleOffset(float angleOffset)
    {
        setAngleOffset = angleOffset;
    }
    private void TargetPlayer()
    {
        if (PlayerCharacter.Instance == null)
        {
            InitializeSetVelocity();
        } else
        {
            Vector2 targetPoint = new Vector2(PlayerCharacter.Instance.transform.position.x, PlayerCharacter.Instance.transform.position.y);
            SetTarget(targetPoint);
        }
    }

    private void InitializeSetVelocity()
    {
        Vector3 vel = new Vector3((components.MMovement.FacingLeft ? -1 : 1) * initialVelocity.x, initialVelocity.y, 0);
        components.MMovement.AddFixedMovements(vel, lifeTime);
    }
    public void SetTarget(Vector2 targetPoint)
    {
        float angle = Mathf.Atan2(targetPoint.y - transform.position.y, targetPoint.x - transform.position.x);
        angle += setAngleOffset;
        Vector2 rawSpeed = new Vector2(Mathf.Cos(angle) * initialVelocity.magnitude, Mathf.Sin(angle) * initialVelocity.magnitude);
        components.MMovement.AddFixedMovements(rawSpeed, lifeTime);
    }
    // Update is called once per frame
    void Update()
    {
        if (!initialized)
        {
            InitializeProjectile();
        }
        if (lifeTime > 0 && components.MScalableTime.TimeSinceLevelLoad() > expirationTime)
        {
            ProjectileDespawn();
        }
    }
    public void ProjectileAttackConfirm(Hitbox hb, Hurtbox hurt, Attackable attackedObj)
    {
        numHits++;
        if (components == null)
        {
            components = GetComponent<CharacterComponents>();
        }
        if (hitSFX != null && components.mAudio != null)
        {
            components.mAudio.PlaySounds(hitSFX);
        }
        if (numHits >= maxHits)
        {
            ProjectileDespawn();
        }
        if (parentAttackConfirm != null)
        {
            parentAttackConfirm.OnAttackConfirm(hb, hurt, attackedObj);
        }
    }
    public void SetParent(AttackConfirm parentAttack)
    {
        parentAttackConfirm = parentAttack;
        
    }

    public void ProjectileDespawn()
    {
        Destroy(gameObject);
    }
}
