using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingProgressBar : MonoBehaviour
{
    Image image;

    private void Awake()
    {
        image = transform.GetComponent<Image>();
    }

    private void Update()
    {
        image.fillAmount = Mathf.Lerp(image.fillAmount, LevelLoader.GetLoadingProgress(), 4f * Time.deltaTime);
    }
}
