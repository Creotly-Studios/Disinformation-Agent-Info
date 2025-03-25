using TMPro;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BiasBingoPanel : GamePanels
{
    // Current Data
    private BingoPostSO currentPost;
    private List<BingoPostSO> dynamicContentList = new();

    [Header("Properties")]
    [SerializeField] private BingoPostSO[] contentArray;

    [Header("Post Properties")]
    [SerializeField] private TextMeshProUGUI authorName;
    [SerializeField] private TextMeshProUGUI postContent;

    private void OnEnable()
    {
        if (hasInitialized) { return; }
        InitalizePanel();
        HandleButtonInitialization(true, ObjectiveType.BiasBingo);
        ShowPanel();
    }

    private void OnDisable()
    {
        if (hasInitialized != true)
        {
            return;
        }
        dynamicContentList.Clear();
        UnInitializePanel(ObjectiveType.BiasBingo);
    }

    #region SO Functions

    private BingoPostSO GetPostSO()
    {
        int random = Random.Range(0, dynamicContentList.Count);
        return dynamicContentList[random];
    }

    protected override void SelectPostSO()
    {
        if(dynamicContentList.Count <= 0)
        {
            Debug.Log("All posts have been answered.");
            EndGame("Congratulations! You've completed all posts!");
            return;
        }

        currentPost = GetPostSO();
        InitializePostContents(currentPost);
        dynamicContentList.Remove(currentPost);

        correctAnswer = currentPost.correctAnswer;
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

    private void InitializePostContents(BingoPostSO postSO)
    {
        postContent.text = postSO.article;
        authorName.text = "Written By : " + postSO.authorName;

        for (int i = 0; i < uiButton.Count; i++)
        {
            uiButton[i].choiceText.text = postSO.options[i];
            uiButton[i].choiceButton.image.color = Color.white;
        }
    }

    #endregion

    protected override IEnumerator ResetCurrentPost()
    {
        hasSet = false;
        selectedAnswer = "";

        yield return waitForSeconds;

        currentPost.hasChecked = true;
        if (currentPost == null || currentPost.hasChecked)
        {
            SelectPostSO();
            uiButton.ForEach(x => x.choiceButton.interactable = true);
        }
    }
}