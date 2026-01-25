public interface IRichTextTag
{
    public string[] Tags { get; }
    
    /// <summary>
    /// Returns text that should replace the tag
    /// </summary>
    /// <param name="tag">data inside arrows ex. link ID=someID </param>
    /// <returns></returns>
    public string OnTag(string tag);
}