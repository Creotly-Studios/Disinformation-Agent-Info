using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MisinformationPanel : GamePanels
{
    [Header("Post Properties")]
    [SerializeField] private Image authorImage;
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
        authorName.text = post.postAuthor;
        postContent.text = post.postContent;
        authorImage.sprite = post.authorImage;

        base.InitializePostContents(post);
    }

    #endregion
}