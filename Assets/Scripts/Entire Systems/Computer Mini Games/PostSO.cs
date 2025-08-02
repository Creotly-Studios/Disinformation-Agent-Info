using System;
using UnityEngine;
using System.Collections.Generic;

public enum PostType
{
    BiasChecker,
    SourceChecker,
    MalignChecker
}

[CreateAssetMenu(fileName = "PostSO", menuName = "Scriptable Objects/PostSO")]
public class PostSO : ScriptableObject
{
    [Header("Status")]
    public bool hasChecked;
    public PostType postType;

    [Header("Author Details")]
    public string postAuthor;
    public Sprite authorImage;

    [Header("Post Parameters")]
    public string postHeader;
    [TextArea] public string postContent;
    [TextArea] public string funFact_Hint;

    [Header("Post Options")]
    public BiasOption[] BiasChoices = new BiasOption[4];
    public MalignOption[] MalignChoices = new MalignOption[2];
    public SourceOption[] SourceChoices = new SourceOption[4];
    public List<OptionBase> PostCheckerOptions { get; private set; }

    public void Initialize()
    {
        PostCheckerOptions = new(GetPostCheckerOptions());
    }

    /// <summary>
    /// Dynamically returns the active options based on post type
    /// </summary>
    /// <returns></returns>
    public OptionBase[] GetPostCheckerOptions()
    {
        switch(postType)
        {
            case PostType.BiasChecker: return Array.ConvertAll(BiasChoices, x => (OptionBase)x);
            case PostType.SourceChecker: return Array.ConvertAll(SourceChoices, x => (OptionBase)x);
            case PostType.MalignChecker: return Array.ConvertAll(MalignChoices, x => (OptionBase)x);
            default: return new OptionBase[0];
        }
    }
}
