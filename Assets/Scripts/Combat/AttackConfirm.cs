using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackConfirm : MonoBehaviour
{
    CharacterComponents components;
    public delegate void OnAttackConfirmEvent(Hitbox hb, Hurtbox hurtbox, Attackable objectHit);
    public OnAttackConfirmEvent attackConfirm;
    // Start is called before the first frame update
    void Start()
    {
        components = GetComponent<CharacterComponents>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnAttackConfirm(Hitbox hb, Hurtbox hurt, Attackable objectHit)
    {
        if (components.MProjectile != null)
        {
            components.MProjectile.ProjectileAttackConfirm(hb, hurt, objectHit);
        }
        if (attackConfirm != null)
        {
            attackConfirm(hb, hurt, objectHit);
        }
    }
}
