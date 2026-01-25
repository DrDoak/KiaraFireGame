using UnityEngine;
using UnityEngine.UI;
public class DialogSpeaker : MonoBehaviour
{
    public Image speaker;
    public Image expression;
    public Animator mAnimator;
    public void DeactivateSpeaker()
    {
        mAnimator.Play("idle", 0, 0);
        gameObject.SetActive(false);
    }
    public void ActivateSpeaker()
    {
        gameObject.SetActive(true);
    }
    public void SetSpeaker(Sprite image, Sprite expImage, string anim = "idle")
    {
        gameObject.SetActive(true);
        speaker.gameObject.SetActive(true);
        expression.gameObject.SetActive(true);

        speaker.sprite = image;
        expression.sprite = expImage;
        speaker.SetNativeSize();
        expression.SetNativeSize();
        if (anim != "holdlast")
        {
            if (anim == "slide_in" || anim == "slide_in_slow")
            {
                mAnimator.Play(anim, 0, 0);
            } else
            {
                mAnimator.CrossFade(anim, 0.15f);
            }
        }
    }
}
