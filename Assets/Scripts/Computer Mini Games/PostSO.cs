using UnityEngine;

[CreateAssetMenu(fileName = "PostSO", menuName = "Scriptable Objects/PostSO")]
public class PostSO : ScriptableObject
{
    [Header("Status")]
    public bool hasChecked;
    public PostType postType;

    [field: SerializeField] public PostFactType postFactType { get; private set; }

    [Header("Details")]
    public Sprite postImage;
    public string postAuthor;
    public Sprite authorImage;
    public string authorUsername;
    [TextArea] public string postContent;

    [Header("Post Parameters")]
    [TextArea] public string funFact_Hint;

}
