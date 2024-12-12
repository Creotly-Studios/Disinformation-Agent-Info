using UnityEngine;
using UnityEngine.UI;

public class LevelSelectButton : MonoBehaviour
{
    private Button _button;

    [SerializeField] private int sceneLoadIndex = 4;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _button = GetComponent<Button>();
        _button.onClick.AddListener(() =>
        {
            LevelLoader.LoadLevel(sceneLoadIndex);
        });
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
