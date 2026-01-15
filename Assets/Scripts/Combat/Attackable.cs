using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Faction { FRIENDLY, ENEMY, NEUTRAL, INVINCIBLE}
public class Attackable : MonoBehaviour
{
    public Faction MyFaction {get {return mFaction;}}
    public CharacterComponents components;
    private Faction mFaction;
    [SerializeField]
    private float maxHP;
    [SerializeField]
    private float currentHP;
    [SerializeField]
    private string deathAnimation;
    public delegate void DeathEvent();
    public DeathEvent OnDeathCallback;
    // Start is called before the first frame update
    void Start()
    {
        components = GetComponent<CharacterComponents>();
        foreach(Hitbox hb in GetComponentsInChildren<Hitbox>())
        {
            hb.SetParent(this);
        }
        foreach (Hurtbox hb in GetComponentsInChildren<Hurtbox>())
        {
            hb.SetParent(this);
        }
    }
    public void TakeHit(Hitbox hb, Hurtbox hurtbox)
    {
        ChangeHP(hb.damage);
        components.MMovement.ApplyImpulse(hb.knockback);
        if (components.MCharacter != null)
        {
            components.MCharacter.ApplyHitStun(hb.hitstunSeconds);
        }
        TimeScaleManager.FreezeTime(hb.hitstopSeconds);
        hb.ParentAttackable.components.MAttackConfirm.OnAttackConfirm(hb, hurtbox, this);
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
        if (deathAnimation != null)
        {
            components.MAnimatorOptions.PlayAnimation(deathAnimation);
        }
    }
}
