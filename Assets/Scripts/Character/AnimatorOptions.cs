using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOptions : MonoBehaviour
{
    public bool hasControl = true;
    public bool canTurn = true;
    public bool cancelOnGrounded;
    public Vector2 applyImpulse;
    private string lastAnimation;
    [SerializeField]
    private string neutralAnimation = "idle";
    [SerializeField]
    private CharacterComponents components;
    [SerializeField]
    private string stepSFX = "step";
    


    public bool InAction { get { return inActionAnimation; } }
    private bool inActionAnimation;
    public delegate void AnimationFinishedAnim(string anim);
    public AnimationFinishedAnim animationFinished;
    private bool startedNewAnimation;
    private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, int> initialSortingSprite = new Dictionary<SpriteRenderer, int>();
    private Vector2 lastApplyImpulse;
    
    

    private void Start()
    {
        sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedNewAnimation && inActionAnimation &&
            (components.mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f ||
            (cancelOnGrounded && components.MMovement.Grounded())))
        {
            inActionAnimation = false;
            if (neutralAnimation == "")
            {
                components.mAnimator.Play(neutralAnimation);
            }
            if (animationFinished != null)
            {
                animationFinished(lastAnimation);
            }
        }
        startedNewAnimation = false;
        if (lastApplyImpulse != applyImpulse)
        {
            if (applyImpulse.magnitude != 0)
            {
                if (applyImpulse.y != 0)
                {
                    components.MMovement.ResetVerticalVelocity();
                }
                Vector2 propel = new Vector2((components.MMovement.FacingLeft ? -1 : 1) * applyImpulse.x, applyImpulse.y);
                components.MMovement.ApplyImpulse(propel);
            }
        }
        lastApplyImpulse = applyImpulse;
        if (components!= null && components.MCharacter != null)
        {
            components.MCharacter.canTurn = canTurn;
        }
    }
    private void LateUpdate()
    {
        //int zVal = Mathf.RoundToInt(-transform.position.z * 100);
        //sprites.ForEach(s => s.sortingOrder = (zVal + initialSortingSprite[s]));
    }
    public void PlayAnimation(string s)
    {
        lastAnimation = s;
        if (components != null && components.mAnimator != null)
        {
            components.mAnimator.speed = 1;
            components.mAnimator.Play(s);
        }
    }
    public void SetNewIdleAnimation(string s, bool playImmediately = false)
    {
        neutralAnimation = s;
        if (playImmediately)
        {
            components.mAnimator.Play(s);
        }
    }
    public void PerformActionAnimation(string s)
    {
        components.mAnimator.speed = 1;
        lastAnimation = s;
        components.mAnimator.Play(s);
        inActionAnimation = true;
        startedNewAnimation = true;
    }

    public void DestroyNow()
    {
        
        Destroy(gameObject);
    }

    public void PlaySFX(string sfx)
    {
        components.mAudio.PlaySounds(sfx);
    }
    public void PlayStepSFX()
    {
        if (stepSFX != "")
        {
            components.mAudio.PlaySounds(stepSFX);
        }
    }
}
