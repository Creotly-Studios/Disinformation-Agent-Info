using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
    static AsyncOperation asyncOperation;
    private static Action onLoaderCallback;
    private class LoadingMonobehavior : MonoBehaviour { }

    public static void LoadLevel(int levelIndex, Action action = null)
    {
        onLoaderCallback = () => {
            GameObject loadingGameobject = new GameObject("Loading Game Object");
            loadingGameobject.AddComponent<LoadingMonobehavior>().StartCoroutine(LoadAsync(levelIndex, action));
        };
        SceneManager.LoadScene("Loading");
    }

    static IEnumerator LoadAsync(int sceneIndex, Action action)
    {
        asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        asyncOperation.allowSceneActivation = true;

        DialogueManager dialogueManager = DialogueManager.Instance;
        if (dialogueManager != null) dialogueManager.EnableDialoguePanel(false);
        while (!asyncOperation.isDone)
        {
            
            GC.Collect();
            yield return null;
        }
        Debug.Log("towe");
        yield return null;

        action?.Invoke();
        if (action == null) SaveManagerSystem.Instance.AutoSave();
    }


    public static float GetLoadingProgress()
    {
        if (asyncOperation != null)
        {
            return asyncOperation.progress;
        } else {
            return 1f;
        }
    }

    public static void LoaderCallback()
    {
        //triggered after the first update which lets the screen refresh
        //excecute the loader callback action to excecute the target scene
        if (onLoaderCallback != null)
        {
           onLoaderCallback();
           onLoaderCallback = null; 
        }
    }

}