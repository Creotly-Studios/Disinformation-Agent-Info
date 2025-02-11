using System;
using UnityEngine;

public class StatsPanel : MonoBehaviour
{
    void Start()
    {
        Hide();
    }

    void Hide()
    {
        gameObject.SetActive(false);
    }

    void Show()
    {
        gameObject.SetActive(true);
    }

}