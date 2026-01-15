using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterComponents : MonoBehaviour
{
    public BaseMovement MMovement;
    public Character MCharacter;
    public Attackable MAttackable;
    public ScalableTime MScalableTime;
    public AttackConfirm MAttackConfirm;
    public AnimatorOptions MAnimatorOptions;
    public Animator mAnimator;

    public Projectile MProjectile;

    // Start is called before the first frame update
    void Awake()
    {
        MMovement = GetComponent<BaseMovement>();
        MCharacter = GetComponent<Character>();
        MAttackable = GetComponent<Attackable>();
        MAttackConfirm = GetComponent<AttackConfirm>();
        MScalableTime = GetComponent<ScalableTime>();
        MProjectile = GetComponent<Projectile>();
        MAnimatorOptions = GetComponentInChildren<AnimatorOptions>();
        mAnimator = GetComponentInChildren<Animator>();
    }
}
