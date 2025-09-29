using TMPro;
using UnityEngine;

public class BiasBingoPanel : GamePanels
{
    [Header("Post Properties")]
    [SerializeField] private TextMeshProUGUI authorName;
    [SerializeField] private TextMeshProUGUI postContent;

    protected override void OnEnable()
    {
        if (hasInitialized)
        { 
            return;
        }
        base.OnEnable();
    }

    protected override void OnDisable()
    {
        if (hasInitialized != true)
        {
            return;
        }
        base.OnDisable();
    }

    #region SO Functions

    public override void InitializePostContents(PostSO post)
    {
        postContent.text = post.postContent;
        authorName.text = "Written By " + post.postAuthor;
        base.InitializePostContents(post);
    }

    #endregion
}