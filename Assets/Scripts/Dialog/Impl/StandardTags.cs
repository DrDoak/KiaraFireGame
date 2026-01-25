using System;
using UnityEngine;

public class Passthrough : IRichTextTag
    {
        public string[] Tags { get; } = { "align","allcaps","alpha","b","br","color","cspace","font","font-weight",
            "gradient","i","indent","line-height","line-indent","link","lowercase","margin","mark","mspace","nobr",
            "noparse","page","pos","rotate","s","size","smallcaps","space","sprite","strikethrough","style","sub",
            "u","uppercase","voffset","width"
        };
        public string OnTag(string tag)
        {
            return $"<{tag}>";

        }
    }

public class DialogEvent : IRichTextTag
{
    public string[] Tags { get; } = { "event", "invoke" };
    
    public string OnTag(string tag)
    {
        DialogManager.InvokeDialogEvent_internal(tag[(tag.IndexOf(" ", StringComparison.Ordinal) + 1)..]);

        return string.Empty;
    }
}

public class TextSpeed : IRichTextTag
{
    public string[] Tags { get; } = { "text-speed", "speed" };

    public string OnTag(string tag)
    {
        int spaceIndex = tag.IndexOf(" ", StringComparison.Ordinal);
        Debug.Assert(spaceIndex != -1);
        float speed = float.Parse(tag[spaceIndex..].Trim());
        DialogManager.Instance.characterTime = speed;
        
        return string.Empty;
    }
}