using UnityEngine;

[CreateAssetMenu(fileName = "dialogSpeaker", menuName = "Holojam/SpeakerInfo", order = 0)]
public class SpeakerInfo : ScriptableObject
{
    public string name;
    public DialogSpeakerPosition DefaultSpeakerPosition = DialogSpeakerPosition.LEFT_SPEAKER;
    public string replacementTypeSFX;
    public Sprite defaultPortrait;

    public bool reverseImage = false;

    public Sprite normalExpression;
    public Sprite happyExpression;
    public Sprite sadExpression;
    public Sprite bigsadExpression;
    public Sprite tiredExpression;
    public Sprite confusedExpression;
    public Sprite smugExpression;
    public Sprite emptyExpression;

    public Sprite GetExpression(DialogExpression exp)
    {
        Sprite expression = null;
        switch (exp)
        {
            case DialogExpression.NORMAL:
                expression = normalExpression;
                break;
            case DialogExpression.HAPPY:
                expression = happyExpression;
                break;
            case DialogExpression.SAD:
                expression = sadExpression;
                break;
            case DialogExpression.BIG_SAD:
                expression = bigsadExpression;
                break;
            case DialogExpression.TIRED:
                expression = tiredExpression;
                break;
            case DialogExpression.CONFUSED:
                expression = confusedExpression;
                break;
            case DialogExpression.SMUG:
                expression = smugExpression;
                break;
            case DialogExpression.EMPTY:
                expression = emptyExpression;
                break;
            default:
                break;
        }
        return expression == null ? normalExpression : expression;
    }
}
public enum DialogExpression { NORMAL, HAPPY, SAD, BIG_SAD, TIRED, CONFUSED, SMUG , EMPTY};