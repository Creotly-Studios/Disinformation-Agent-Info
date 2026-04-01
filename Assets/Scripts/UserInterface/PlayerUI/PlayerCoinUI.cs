using System;
using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerCoinUI : MonoBehaviour
{
    [SerializeField] private float displayTime = 3f;
    [SerializeField] private TextMeshProUGUI coinAmountText;
    [SerializeField] private GameObject coinUIPanel;

    private Coroutine hideCoroutine;

    private void OnEnable()
    {
        // Subscribe here so the listener is properly removed if the object is disabled.
        // Was in Start() with no corresponding unsubscribe — listener would stack on re-enable.
        if (Player_v2.Instance != null)
            Player_v2.Instance.OnCollectCoin += OnCoinCollected;
    }

    private void OnDisable()
    {
        if (Player_v2.Instance != null)
            Player_v2.Instance.OnCollectCoin -= OnCoinCollected;
    }

    private void Start() => Show();

    private void OnCoinCollected(object sender, EventArgs e) => Show();

    public void Show()
    {
        // Fix: was GameManager.Instance.PlayerCoins() — method removed, property is PlayerCoinAmount.
        coinAmountText.text = GameManager.Instance.PlayerCoinAmount.ToString();
        coinUIPanel.SetActive(true);

        // Cancel any in-progress hide so the timer resets on rapid pickups.
        if (hideCoroutine != null) StopCoroutine(hideCoroutine);
        hideCoroutine = StartCoroutine(DisplayAndHide());
    }

    private IEnumerator DisplayAndHide()
    {
        yield return new WaitForSeconds(displayTime);
        coinUIPanel.SetActive(false);
        hideCoroutine = null;
    }
}