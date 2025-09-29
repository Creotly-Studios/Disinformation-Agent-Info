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
        var dialogueManager = DialogueManager.Instance;
        if (dialogueManager != null)
        {
            dialogueManager.EnableDialoguePanel(false);
        }

        var gm = GameManager.Instance;
        if (gm != null)    
        {
            gm.UnPause();
        }

        var op = SceneManager.LoadSceneAsync(_sceneIndex);
        if (op == null)
        {
            Finish();
            yield break;
        }

        op.allowSceneActivation = true;
        LevelLoader.SetCurrentAsyncOperation(op);

        while (!op.isDone)
        {
            yield return null;
        }
        yield return null;

        _onComplete?.Invoke();
        Finish();
    }

    private void Finish()
    {
        Destroy(gameObject);
    }
}


public static class LevelLoader
{
    private static int targetSceneIndex = -1;
    private static Action onComplete;
    private static AsyncOperation asyncOperation;

    /// <summary>
    /// Call to begin loading a target scene via the "Loading" scene.
    /// </summary>
    public static void LoadLevel(int levelIndex, Action action = null)
    {
        onComplete = action;
        targetSceneIndex = levelIndex;

        SceneManager.LoadScene("Loading");
        SceneManager.sceneLoaded -= OnSceneLoaded;
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name != "Loading") return;
        SceneManager.sceneLoaded -= OnSceneLoaded;

        var go = new GameObject("LevelLoaderRunner");
        GameObject.DontDestroyOnLoad(go);
        var runner = go.AddComponent<LevelLoaderRunner>();
        runner.BeginLoad(targetSceneIndex, OnLoadCompleteCallback);
    }

    private static void OnLoadCompleteCallback()
    {
        onComplete?.Invoke();
        if (onComplete == null)
        {
            var save = SaveManagerSystem.Instance;
            if (save != null) save.AutoSave();
        }
        onComplete = null;
        targetSceneIndex = -1;
        asyncOperation = null;
    }

    public static float GetLoadingProgress()
    {
        if (asyncOperation != null) return asyncOperation.progress;
        return 1f;
    }

    internal static void SetCurrentAsyncOperation(AsyncOperation op) => asyncOperation = op;
}
