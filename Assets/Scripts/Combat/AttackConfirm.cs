using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackConfirm : MonoBehaviour
{
    CharacterComponents components;
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
    }
}
