using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Faction { FRIENDLY, ENEMY, NEUTRAL, INVINCIBLE}
public class Attackable : MonoBehaviour
{
    public Faction MyFaction {get {return mFaction;} set { mFaction = value; } }
    public CharacterComponents components;
    [SerializeField]
    private Faction mFaction;
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float currentHP;
    [SerializeField]
    private string deathAnimation;
    [SerializeField]
    private float knockbackScale = 1;
    public delegate void DeathEvent();
    public DeathEvent OnDeathCallback;
    // Start is called before the first frame update
    void Start()
    {
        components = GetComponent<CharacterComponents>();
        foreach(Hitbox hb in GetComponentsInChildren<Hitbox>(includeInactive: true))
        {
            hb.SetParent(this);
        }
        foreach (Hurtbox hb in GetComponentsInChildren<Hurtbox>(includeInactive: true))
        {
            hb.SetParent(this);
        }
    }
    public void TakeHit(Hitbox hb, Hurtbox hurtbox)
    {
        if (!components.MCharacter.canBlock || hb.unblockable)
        {
            ChangeHP(-hb.damage);
        }
        bool facingLeft = false;
        if (hb.ParentAttackable != null)
        {
            facingLeft = hb.ParentAttackable.components.MMovement.FacingLeft;
        }
        Vector2 knockback = new Vector2((facingLeft ? -1 : 1) * hb.knockback.x, hb.knockback.y);
        if (components.MCharacter.canBlock)
        {
            knockback *= 0f;
        }
        knockback *= knockbackScale;
        components.MMovement.ApplyImpulse(knockback);
        if (components.MCharacter != null)
        {
            components.MCharacter.ApplyHitStun(hb.hitstunSeconds);
        }
        TimeScaleManager.FreezeTime(hb.hitstopSeconds);
        if (hb.ParentAttackable != null)
        {
            hb.ParentAttackable.components.MAttackConfirm.OnAttackConfirm(hb, hurtbox, this);
        }
    }
    public void ChangeHP(float delta)
    {
        currentHP = Mathf.Clamp(currentHP + delta, 0, maxHP);
        if (currentHP == 0)
        {
            OnDeath();
        }
    }

    public void OnDeath()
    {
        if (OnDeathCallback != null)
        {
            OnDeathCallback();
        }
        if (deathAnimation != "")
        {
            components.MAnimatorOptions.PlayAnimation(deathAnimation);
        } else
        {
            Destroy(gameObject);
        }
    }
}
