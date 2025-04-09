using System.Collections;
using Garawell.Managers.Events;
using Garawell.Utility;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Garawell.Managers.Scene
{
    public class LevelLoader : MonoBehaviour
    {
        public LevelManager levelManager;

        private void Awake()
        {
            levelManager.Initialize();
        }

        public void LoadLevel(SceneReference levelScene, LoadSceneMode mode, UnityAction callback)
        {
            StartCoroutine(LoadLevelRoutine(levelScene, mode, callback));
        }

        public IEnumerator LoadLevelRoutine(SceneReference prefabLevelScene, LoadSceneMode mode, UnityAction callback)
        {
            AsyncOperation async = SceneManager.LoadSceneAsync(prefabLevelScene, mode);
            while (!async.isDone)
            {
                yield return null;
            }
            callback?.Invoke();
        }
        public void UnloadLevel()
        {
            StartCoroutine(UnloadLevelRoutine());
        }

        public IEnumerator UnloadLevelRoutine()
        {
            yield return new WaitForSeconds(1f);
            AsyncOperation async = SceneManager.UnloadSceneAsync("LevelLoaderScene");
            EventRunner.LoadSceneFinish();
        }
    }
}

