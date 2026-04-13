using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LevelLoaderRunner : MonoBehaviour
{
    private int _sceneIndex;
    private Action _onComplete;

    public void BeginLoad(int sceneIndex, Action onComplete)
    {
        _sceneIndex = sceneIndex;
        _onComplete = onComplete;
        StartCoroutine(LoadAsync());
    }

    private IEnumerator LoadAsync()
    {
        if (DialogueManager.Instance != null)
            DialogueManager.Instance.EnableDialoguePanel(false);

        if (GameManager.Instance != null)
            GameManager.Instance.UnPause();

        AsyncOperation op = SceneManager.LoadSceneAsync(_sceneIndex);
        if (op == null)
        {
            Finish();
            yield break;
        }

        op.allowSceneActivation = true;
        LevelLoader.SetCurrentAsyncOperation(op);

        while (!op.isDone)
            yield return null;

        yield return null;

        _onComplete?.Invoke();
        Finish();
    }

    private void Finish() => Destroy(gameObject);
}

public static class LevelLoader
{
    private static int targetSceneIndex = -1;
    private static Action onComplete;
    private static AsyncOperation asyncOperation;

    /// <summary>Loads a target scene via the "Loading" scene.</summary>
    public static void LoadLevel(int levelIndex, Action action = null)
    {
        onComplete = action;
        targetSceneIndex = levelIndex;

        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.LoadScene("Loading");
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Loading") return;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        GameObject go = new("LevelLoaderRunner");
        GameObject.DontDestroyOnLoad(go);
        go.AddComponent<LevelLoaderRunner>().BeginLoad(targetSceneIndex, OnLoadCompleteCallback);
    }

    private static void OnLoadCompleteCallback()
    {
        onComplete?.Invoke();
        EventBus.Gameplay.OnNewSceneLoaded?.Invoke(false);

        if (onComplete == null)
        {
            EventBus.Save.OnHandleAutoSave?.Invoke();
        }
        onComplete = null;
        targetSceneIndex = -1;
        asyncOperation = null;
    }

    public static float GetLoadingProgress() => asyncOperation?.progress ?? 1f;

    internal static void SetCurrentAsyncOperation(AsyncOperation op) => asyncOperation = op;
}