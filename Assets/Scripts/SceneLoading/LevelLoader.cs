using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class LevelLoader
{
    static AsyncOperation asyncOperation;
    private static Action onLoaderCallback;
    private class LoadingMonobehavior : MonoBehaviour { }

    public static void LoadLevel(int levelIndex)
    {
        onLoaderCallback = () => {
            GameObject loadingGameobject = new GameObject("Loading Game Object");
            loadingGameobject.AddComponent<LoadingMonobehavior>().StartCoroutine(LoadAsync(levelIndex));
        };

        SceneManager.LoadScene("Loading");
    }

    static IEnumerator LoadAsync(int sceneIndex)
    {
        asyncOperation = SceneManager.LoadSceneAsync(sceneIndex);
        while (!asyncOperation.isDone)
        {
            // Debug.Log(asyncOperation.progress);
            GC.Collect();
            yield return null;
        }
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