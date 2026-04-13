using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Action = System.Action;

public class NoticePopup : MonoBehaviour
{
    private bool isDismissing;
    private bool hasInitialized;
    private static readonly WaitForSeconds AutoDismiss = new(2.0f);

    [Header("Buttons")]
    [SerializeField] private Button[] progressButton;
    [SerializeField] private TextMeshProUGUI[] buttonText;
    [SerializeField] private GameObject buttonsContainer;

    [Header("Text")]
    [SerializeField] private TextMeshProUGUI titleText;
    [SerializeField] private TextMeshProUGUI contentText;

    public void SubscribeEvents()
    {
        if (hasInitialized)
        {
            return;
        }
        hasInitialized = true;
        EventBus.Notification.OnShow += HandleShow;
        EventBus.Notification.OnDismiss += HandleDismiss;
    }

    public void UnSubscribeEvents()
    {
        if (hasInitialized != true)
        {
            return;
        }

        hasInitialized = false;
        EventBus.Notification.OnShow -= HandleShow;
        EventBus.Notification.OnDismiss -= HandleDismiss;
        foreach (Button btn in progressButton)
        {
            btn.onClick.RemoveAllListeners();
        }
    }
    private void HandleShow(NoticePopup target, NotificationRequest request)
    {
        if (target != this)
        {
            return;
        }
        StopAllCoroutines();

        Dispatch(request);
    }

    private void HandleDismiss(NoticePopup target)
    {
        if (target != this)
        {
            return;
        }
        Dismiss();
    }

    // ── Dispatch ──────────────────────────────────────────────────────────────

    private void Dispatch(NotificationRequest r)
    {
        switch (r.Type)
        {
            case NoticeType.QuestCompleted:
                gameObject.SetActive(true);
                StartCoroutine(TimedBanner(r.Duration,
                    r.Quest.isComplete ? "Quest Completed" : "New Mission",
                    r.Quest.questTitle,
                    r.Quest.isComplete ? Color.green : Color.white));
                break;

            case NoticeType.ObjectiveCompleted:
                gameObject.SetActive(true);
                StartCoroutine(TimedBanner(r.Duration,
                    r.Objective.isDone ? "Objective Completed" : "New Objective",
                    r.Objective.description,
                    r.Objective.isDone ? Color.green : Color.white));
                break;

            case NoticeType.Dialogue:
                gameObject.SetActive(true);
                StartCoroutine(TimedBanner(0f, "Notice", r.Body, r.TextColor));
                break;

            case NoticeType.Correct:
            case NoticeType.Wrong:
                ShowQuizResult(r);
                break;

            case NoticeType.Hint:
                ShowSingleButton(" ", r.Body, "Continue");
                break;

            case NoticeType.GameOver:
                ShowTwoButton("GAME OVER !!!", r.Body, Color.red,
                    r.PrimaryLabel, () => { Dismiss(); r.PrimaryAction?.Invoke(); },
                    r.SecondaryLabel, () => { Dismiss(); r.SecondaryAction?.Invoke(); });
                break;

            case NoticeType.Payment:
                ShowPayment(r);
                break;

            case NoticeType.Confirm:
                ShowTwoButton(string.Empty, r.Body, Color.white,
                    r.PrimaryLabel, () => { Dismiss(); r.PrimaryAction?.Invoke(); },
                    r.SecondaryLabel, () => { Dismiss(); r.SecondaryAction?.Invoke(); });
                break;
        }
    }

    private IEnumerator TimedBanner(float delay, string title, string body, Color bodyColor)
    {
        if (delay > 0f) yield return new WaitForSeconds(delay);
        yield return null; // One frame for layout to settle.

        contentText.color = bodyColor;
        SetText(title, body);

        yield return AutoDismiss;
        Dismiss();
    }

    private void ShowQuizResult(NotificationRequest r)
    {
        bool correct = r.Type == NoticeType.Correct;
        Color color = correct ? Color.green : Color.red;
        string title = correct ? "You are Correct" : "You are Incorrect";

        titleText.color = color;
        contentText.color = color;
        ShowSingleButton(title, r.Body, "Continue");
    }

    private void ShowPayment(NotificationRequest r)
    {
        gameObject.SetActive(true);
        buttonsContainer.SetActive(true);
        SetText("Payment Needed", r.Body);
        SetButtonVisibility(true, true);
        PrepButton(r.PrimaryLabel, 0, () => ProcessPayment(r));
        PrepButton(r.SecondaryLabel, 1, Dismiss);
    }

    private void ProcessPayment(NotificationRequest r)
    {
        GameManager gm = GameManager.Instance;
        int coins = gm.PlayerCoinAmount;
        bool sufficient = coins >= r.CoinCost;
        if (sufficient) gm.SetCoinAmount(coins - r.CoinCost);
        StartCoroutine(PaymentResult(sufficient, r.PrimaryAction));
    }

    private IEnumerator PaymentResult(bool success, Action onSuccess)
    {
        buttonsContainer.SetActive(false);
        SetText("Payment Status", success ? "Payment Successful" : "Insufficient Funds");
        yield return AutoDismiss;
        Dismiss();
        if (success) onSuccess?.Invoke();
    }

    private void ShowSingleButton(string title, string body, string btnLabel)
    {
        gameObject.SetActive(true);
        SetText(title, body);
        SetButtonVisibility(true, false);
        PrepButton(btnLabel, 0, Dismiss);
    }

    private void ShowTwoButton(string title, string body, Color titleColor,
        string primaryLabel, Action primaryAction, string secondaryLabel, Action secondaryAction)
    {
        gameObject.SetActive(true);
        SetTextColor(titleText, titleColor);
        SetTextColor(contentText,titleColor);
        SetText(title, body);
        SetButtonVisibility(true, true);
        PrepButton(primaryLabel, 0, primaryAction);
        PrepButton(secondaryLabel, 1, secondaryAction);
    }

    private void SetButtonVisibility(bool first, bool second)
    {
        if(progressButton.Length < 1)
        {
            return;
        }

        progressButton[0].gameObject.SetActive(first);
        if (progressButton.Length > 1)
        {
            progressButton[1].gameObject.SetActive(second);
        }
    }

    private void PrepButton(string label, int index, Action onClick)
    {
        progressButton[index].onClick.RemoveAllListeners();
        progressButton[index].onClick.AddListener(() => onClick?.Invoke());
        buttonText[index].text = label;
        progressButton[index].gameObject.SetActive(true);
    }

    private void SetText(string title, string body)
    {
        if(titleText != null)
        {
            titleText.text = title;
        }
        contentText.text = body;
    }

    private void SetTextColor(TextMeshProUGUI text, Color color)
    {
        if(text != null)
        {
            text.color = color;
        }
    }

    private void Dismiss()
    {
        if (isDismissing)
        {
            return;
        }
        isDismissing = true;
        StopAllCoroutines();
        EventBus.Notification.OnDismiss?.Invoke(this); // Notify subscribers (e.g. ComputerPanel_UI) before deactivating.
        gameObject.SetActive(false);
        isDismissing = false;
    }
}