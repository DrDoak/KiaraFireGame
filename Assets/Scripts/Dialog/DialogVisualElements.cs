using UnityEngine;
using UnityEngine.UI;
using FMODUnity;
public enum DialogSpeakerAnimation { IDLE, FACEAWAY, HOP, HIT, SHAKE, LOW, GET_CLOSER, GET_VERYCLOSE, BACK_OFF, SLIDE_IN, HOLD_LAST, SLIDE_IN_SLOW, RUN_AROUND, OFFSCREEN };
public class DialogVisualElements : MonoBehaviour
{
    public static DialogVisualElements Instance { get; private set; } //
    [SerializeField]
    private DialogSpeaker leftSpeaker;
    [SerializeField]
    private DialogSpeaker rightSpeaker;

    
    [SerializeField]
    private Transform cutsceneTransform;
    [SerializeField]
    private Image showCutsceneImage;

    private bool playingAnim;
    private Animator mCutsceneAnimator;
    private GameObject lastCutscenePrefab;
    private string lastCutsceneAnimPlaying;
    private float timeExpire;
    private bool lastExtraLayer = false;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }
    public static void ProcessCutscene(DialogCutsceneSettings cutsceneSettings)
    {
        if (Instance == null) return;
        Instance.ProcessMusic(cutsceneSettings);
        if (cutsceneSettings.forcePause > 0)
        {
            Instance.timeExpire = Time.timeSinceLevelLoad + cutsceneSettings.forcePause;
            cutsceneSettings.dialogPlayOption = DialogSkipOption.AUTO_ADVANCE_ONLY;
        }
        Instance.ProcessShowImage(cutsceneSettings);
        Instance.playingAnim = false;

        if (cutsceneSettings.showObject == null)
        {
            Instance.DeleteCutsceneObj();
            return;
        }
        if (Instance.lastCutscenePrefab != cutsceneSettings.showObject)
        {
            Instance.DeleteCutsceneObj();
            Instance.mCutsceneAnimator = Instantiate(cutsceneSettings.showObject, Instance.cutsceneTransform).GetComponent<Animator>();
            Instance.lastCutscenePrefab = cutsceneSettings.showObject;
        }
        if (cutsceneSettings.playAnimation != "" &&
            cutsceneSettings.playAnimation != Instance.lastCutsceneAnimPlaying)
        {
            Instance.playingAnim = true;
            Instance.mCutsceneAnimator.Play(cutsceneSettings.playAnimation, 0, 0);
            Instance.mCutsceneAnimator.Update(0);
            Instance.lastCutsceneAnimPlaying = cutsceneSettings.playAnimation;
        }
        
    }
    
    private void ProcessMusic(DialogCutsceneSettings cutsceneSettings)
    {
        if (cutsceneSettings.stopMusic)
        {
            AudioManager.StopAllMusic();            
        }
        if (!cutsceneSettings.startMusicEvent.IsNull)
        {
            AudioManager.PlayMusic(cutsceneSettings.startMusicEvent);
        }
        if (cutsceneSettings.stopAmbience )
        {
            AudioManager.StopAllAmbience();            
        }
        if (!cutsceneSettings.ambienceEvent.IsNull)
        {
            AudioManager.PlayAmbience(cutsceneSettings.ambienceEvent);
        }
    }
    
    public static void DialogClose()
    {
        if (Instance == null) return;
        Instance.leftSpeaker.DeactivateSpeaker();
        Instance.rightSpeaker.DeactivateSpeaker();
        Instance.DeleteCutsceneObj();
    }
    private void ProcessShowImage(DialogCutsceneSettings cutsceneSettings)
    {
        if (cutsceneSettings.showImage == null)
        {
            showCutsceneImage.gameObject.SetActive(false);
        } else
        {
            showCutsceneImage.gameObject.SetActive(true);
            showCutsceneImage.sprite = cutsceneSettings.showImage;
            showCutsceneImage.SetNativeSize();
        }
    }

    private void DeleteCutsceneObj()
    {
        if (Instance.mCutsceneAnimator != null)
        {
            Destroy(Instance.mCutsceneAnimator.gameObject);
            Instance.mCutsceneAnimator = null;
            Instance.lastCutscenePrefab = null;
        }
        Instance.lastCutsceneAnimPlaying = "";
    }
    public static void ProcessVisualElements(DialogElements elements)
    {
        if (elements.clearAllPreviousPortraits)
        {
            Instance.leftSpeaker.DeactivateSpeaker();
            Instance.rightSpeaker.DeactivateSpeaker();
        }
        if (elements.speaker == null) return;
        DialogSpeakerPosition pos = (elements.overrideSpeakerPosition == DialogSpeakerPosition.DEFAULT ?  elements.speaker.DefaultSpeakerPosition : elements.overrideSpeakerPosition);
        DialogSpeaker activeSpeaker;
        if (pos == DialogSpeakerPosition.LEFT_SPEAKER)
        {
            activeSpeaker = Instance.leftSpeaker;
        } else
        {
            activeSpeaker = Instance.rightSpeaker;
        }
        activeSpeaker.ActivateSpeaker();
        Sprite portrait = elements.overridePortrait == null ? elements.speaker.defaultPortrait : elements.overridePortrait;
        Sprite expression = elements.speaker.GetExpression(elements.expression);
        string anim = Instance.SpeakerAnimation(elements.playSpeakerAnimation);
        activeSpeaker.SetSpeaker(portrait, expression, anim);
        
    }
    private string SpeakerAnimation(DialogSpeakerAnimation anim)
    {
        switch (anim)
        {
            case DialogSpeakerAnimation.IDLE:
                return "idle";
            case DialogSpeakerAnimation.FACEAWAY:
                return "faceaway";
            case DialogSpeakerAnimation.HOP:
                return "hop";
            case DialogSpeakerAnimation.HIT:
                return "hit";
            case DialogSpeakerAnimation.SHAKE:
                return "shake";
            case DialogSpeakerAnimation.LOW:
                return "low";
            case DialogSpeakerAnimation.HOLD_LAST:
                return "holdlast";
            case DialogSpeakerAnimation.GET_CLOSER:
                return "getcloser";
            case DialogSpeakerAnimation.GET_VERYCLOSE:
                return "getveryclose";
            case DialogSpeakerAnimation.BACK_OFF:
                return "backoff";
            case DialogSpeakerAnimation.SLIDE_IN:
                return "slide_in";
            case DialogSpeakerAnimation.SLIDE_IN_SLOW:
                return "slide_in_slow";
            case DialogSpeakerAnimation.RUN_AROUND:
                return "runaround";
            case DialogSpeakerAnimation.OFFSCREEN:
                return "offscreen";
            default:
                return "holdlast";
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (DialogManager.IsDialogPlaying &&
            DialogManager.Instance.CurrentDialog.dialogSettings.cutsceneOptions.forcePause > 0)
        {
            if (Time.timeSinceLevelLoad > timeExpire)
            {
                DialogManager.Instance.Next();
            }
        }
        if (playingAnim && DialogManager.IsDialogPlaying &&
            DialogManager.Instance.CurrentDialog.dialogSettings.cutsceneOptions.dialogPlayOption == DialogSkipOption.AUTO_ADVANCE_ONLY)
        {
            if (mCutsceneAnimator != null)
            {
                if (mCutsceneAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.99f)
                {
                    DialogManager.Instance.Next();
                }
            } else
            {
                DialogManager.Instance.Next();
            }
        }
    }
}
