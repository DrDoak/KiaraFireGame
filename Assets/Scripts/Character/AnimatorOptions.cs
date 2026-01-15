using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimatorOptions : MonoBehaviour
{
    public bool hasControl = true;
    public bool canTurn = true;
    private Animator mAnimator;
    private string lastAnimation;
    [SerializeField]
    private string neutralAnimation = "idle";
    [SerializeField]
    private CharacterComponents components;

    public bool InAction { get { return inActionAnimation; } }
    private bool inActionAnimation;
    public delegate void AnimationFinishedAnim(string anim);
    public AnimationFinishedAnim animationFinished;
    private bool startedNewAnimation;
    private List<SpriteRenderer> sprites = new List<SpriteRenderer>();
    private Dictionary<SpriteRenderer, int> initialSortingSprite = new Dictionary<SpriteRenderer, int>();

    private void Start()
    {
        mAnimator.GetComponent<Animator>();
        sprites = new List<SpriteRenderer>(GetComponentsInChildren<SpriteRenderer>());
        mAnimator.Play(neutralAnimation, 0, Random.Range(0, 1f));
        sprites.ForEach(s => initialSortingSprite[s] = s.sortingOrder);
    }

    // Update is called once per frame
    void Update()
    {
        if (!startedNewAnimation && inActionAnimation && mAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.95f)
        {
            inActionAnimation = false;
            if (neutralAnimation == "")
            {
                mAnimator.Play(neutralAnimation);
            }
            if (animationFinished != null)
            {
                animationFinished(lastAnimation);
            }
        }
        startedNewAnimation = false;

    }
    private void LateUpdate()
    {
        int zVal = Mathf.RoundToInt(-transform.position.z * 100);
        sprites.ForEach(s => s.sortingOrder = (zVal + initialSortingSprite[s]));
    }
    public void PlayAnimation(string s)
    {
        lastAnimation = s;
        mAnimator.Play(s);
    }
    public void SetNewIdleAnimation(string s, bool playImmediately = false)
    {
        neutralAnimation = s;
        if (playImmediately)
        {
            mAnimator.Play(s);
        }
    }
    public void PerformActionAnimation(string s)
    {
        lastAnimation = s;
        mAnimator.Play(s);
        inActionAnimation = true;
        startedNewAnimation = true;
    }

    public void DestroyNow()
    {
        Destroy(gameObject);
    }
}
