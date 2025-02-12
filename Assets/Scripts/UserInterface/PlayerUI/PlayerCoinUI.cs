using System;
using UnityEngine;
using System.Collections;
using TMPro;

public class PlayerCoinUI : MonoBehaviour
{
    [SerializeField] float displayTime = 3f;
    [SerializeField] TextMeshProUGUI coinAmountText;
    [SerializeField] GameObject coinUIPanel;

    void Start()
    {
        // Player_v2.Instance.OnCollectCoin += Player_OnCollectCoin;
        Show();
    }

    private void Player_OnCollectCoin(object sender, EventArgs e)
    {
        Show();
    }

    public void Show()
    {
        StartCoroutine(DisplayAndHide());
    }

    private IEnumerator DisplayAndHide()
    {
        coinUIPanel.SetActive(true);
        // coinAmountText.text = Player_v2.Instance.coinAmount;

        yield return new WaitForSeconds(displayTime);
        coinUIPanel.SetActive(false);
    }
}
