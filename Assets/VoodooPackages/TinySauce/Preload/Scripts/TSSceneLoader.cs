using Garawell.Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class TSSceneLoader : MonoBehaviour
{
    private void Start()
    {
        // SDK always enabled on builds, and will not be enabled in the editor
#if UNITY_EDITOR
        StartCoroutine(UnloadScenes());
#else
        TinySauce.SubscribeOnInitFinishedEvent(LoadScene);
#endif
    }

    private void LoadScene(bool adConsent, bool trackingConsent)
    {
        StartCoroutine(UnloadScenes());
    }

    public IEnumerator UnloadScenes()
    {
        bool managerSceneExist = false;
        AsyncOperation async;

        int z = 1;
        for (int i = 0; i < SceneManager.sceneCount - 1; i++)
        {
            if (!SceneManager.GetSceneAt(z).name.Equals("ManagerScene"))
            {
                async = SceneManager.UnloadSceneAsync(SceneManager.GetSceneAt(z));
                while (!async.isDone)
                {
                    yield return null;
                }
            }
            else
            {
                z++;
                managerSceneExist = true;
            }
        }
        if (!managerSceneExist)
        {
            async = SceneManager.LoadSceneAsync("ManagerScene", LoadSceneMode.Additive);
            while (!async.isDone)
            {
                yield return null;
            }
        }
        MainManager.Instance.Initialize();
        SceneManager.UnloadSceneAsync("TSPreloadScene");
    }
}