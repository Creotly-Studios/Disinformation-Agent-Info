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

    private void Start()
    {
        Player_v2.Instance.OnCollectCoin += OnCoinCollected;
        Show();
    }

    private void OnDestroy()
    {
        if(Player_v2.Instance != null)
        {
            Player_v2.Instance.OnCollectCoin -= OnCoinCollected;
        }
    }

    private void OnCoinCollected(object sender, EventArgs e) => Show();

    public void Show()
    {
        coinAmountText.text = GameManager.Instance.PlayerCoinAmount.ToString();
        coinUIPanel.SetActive(true);

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