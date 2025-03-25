using TMPro;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class MisinformationPanel : GamePanels
{
    //Parameters
    private PostSO currentPost;
    public PostFactType selectedPostType { get; private set; }
    private List<PostSO> dynamicContentList = new List<PostSO>();

    [Header("Properties")]
    [SerializeField] private PostSO[] contentArray;

    [Header("Idenifer Buttons")]
    [SerializeField] private Button info_Btn;
    [SerializeField] private Button malInfo_Btn;

    [Header("Post Properties")]
    [SerializeField] private Image authorImage;
    [SerializeField] private TextMeshProUGUI userName;
    [SerializeField] private TextMeshProUGUI authorName;
    [SerializeField] private TextMeshProUGUI postContent;

    private void OnEnable()
    {
        if (hasInitialized) { return; }
        InitalizePanel();
        HandleButtonInitialization(true, ObjectiveType.MisInfoGames);
        ShowPanel();
    }

    private void OnDisable()
    {
        if (hasInitialized != true) { return; }

        dynamicContentList.Clear();
        UnInitializePanel(ObjectiveType.MisInfoGames);
    }

    #region SO Functions
    private PostSO GetPost()
    {
        int random = Random.Range(0, dynamicContentList.Count);
        return dynamicContentList[random];
    }

    protected override void SelectPostSO()
    {
        if (dynamicContentList.Count == 0)
        {
            EndGame("Congratulations! You've completed all posts!");
            return;
        }

        info_Btn.interactable = true;
        malInfo_Btn.interactable = true;

        currentPost = GetPost();
        InitializePostContents(currentPost);
        dynamicContentList.Remove(currentPost);
        reasonForAnswer = currentPost.answerExplanation;
    }

    protected override void InitalizePosts()
    {
        dynamicContentList = contentArray.ToList();
        for (int i = 0; i < dynamicContentList.Count; i++)
        {
            dynamicContentList[i] = Instantiate(dynamicContentList[i]);
        }
    }

    private void InitializePostContents(PostSO post)
    {
        authorName.text = post.postAuthor;
        postContent.text = post.postContent;
        userName.text = post.authorUsername;
        authorImage.sprite = post.authorImage;
    }

    #endregion

    #region Button Setters

    private void InitalizeButton(Button button, PostFactType factType)
    {
        if (isGameOver) return;

        selectedPostType = factType;
        button.interactable = false;

        if (hasSet == true)
        {
            return;
        }

        hasSet = true;
        if (currentPost.postFactType == selectedPostType)
        {
            CorrectAnswer(currentPost.answerExplanation, ObjectiveType.MisInfoGames);
            return;
        }
        computerPanelUI.popupPanel.DisplayPopUpWindow(currentPost.answerExplanation, NoticeType.Wrong);
    }

    protected override void HandleButtonInitialization(bool status, ObjectiveType objectiveType)
    {
        if(status)
        {
            info_Btn.onClick.AddListener(() => InitalizeButton(info_Btn, PostFactType.Information));
            malInfo_Btn.onClick.AddListener(() => InitalizeButton(malInfo_Btn, PostFactType.MalignedInformation));
            return;
        }
        info_Btn.onClick.RemoveListener(() => InitalizeButton(info_Btn, PostFactType.Information));
        malInfo_Btn.onClick.RemoveListener(() => InitalizeButton(malInfo_Btn, PostFactType.MalignedInformation));
    }

    #endregion

    protected override void EnableButtons(bool status)
    {
        info_Btn.interactable = status;
        malInfo_Btn.interactable = status;
    }

    protected override IEnumerator ResetCurrentPost()
    {
        hasSet = false;
        selectedPostType = PostFactType.None;
        yield return waitForSeconds;

        currentPost.hasChecked = true;
        if (currentPost == null || currentPost.hasChecked)
        {
            SelectPostSO();
        }
    }
}