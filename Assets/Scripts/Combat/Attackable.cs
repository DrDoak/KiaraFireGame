using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum Faction { FRIENDLY, ENEMY, NEUTRAL, INVINCIBLE}
public class Attackable : MonoBehaviour
{
    public Faction MyFaction {get {return mFaction;} set { mFaction = value; } }
    public float MaxHP { get { return maxHP; } set { maxHP = value; } }
    public float CurrentHP { get { return currentHP; } set { currentHP = value; } }
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
    private string deathSFX;
    [SerializeField]
    private float knockbackScale = 1;
    [SerializeField]
    private GameObject defaultHitVFX;
    [SerializeField]
    private GameObject defaultBlockVFX;
    [SerializeField]
    private GameObject deathVFX;
    [SerializeField]
    private bool registerAsEnemyTarget;
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
        if (registerAsEnemyTarget)
        {
            GameManager.RegisterEnemyTarget();
        }
    }
    public void TakeHit(Hitbox hb, Hurtbox hurtbox)
    {
        if (!components.MCharacter.canBlock || hb.unblockable)
        {
            ChangeHP(-hb.damage);
            TimeScaleManager.FreezeTime(hb.hitstopSeconds);
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
        if (hb.ParentAttackable != null)
        {
            hb.ParentAttackable.components.MAttackConfirm.OnAttackConfirm(hb, hurtbox, this);
        }
        if (!components.MCharacter.canBlock || hb.unblockable)
        {
            SpawnVFX(hb, hurtbox, hb.uniqueHitFX == null ? defaultHitVFX : hb.uniqueHitFX, knockback);
        } else
        {
            SpawnVFX(hb, hurtbox, defaultBlockVFX, knockback);
        }
    }

    private void SpawnVFX(Hitbox hb, Hurtbox hurt, GameObject vfx, Vector2 knockback)
    {
        float angle = Mathf.Atan2(knockback.y, knockback.x);
        Vector3 pos = (hb.transform.position + hurt.transform.position) / 2;
        GameObject go = Instantiate(vfx, pos, Quaternion.EulerAngles(0,0,angle));
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
            if (deathSFX != "")
            {
                components.mAudio.PlaySounds(deathSFX);
            }
            if (registerAsEnemyTarget)
            {
                GameManager.RegisterAsDefeated();
            }
            if (deathVFX != null)
            {
                Instantiate(deathVFX, transform.position, Quaternion.identity);
            }
            Destroy(gameObject);
        }
    }
}
