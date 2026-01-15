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
    private Vector2 initialVelocity;

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
            components.MMovement.AddFixedMovements(initialVelocity, lifeTime);
        }
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
