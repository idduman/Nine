using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Garawell.Managers.Scene
{
    public class SceneUnloader : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            //StartCoroutine(UnloadScenes());
        }

        public IEnumerator UnloadScenes()
        {
            bool managerSceneExist = false;
            AsyncOperation async;

            int z = 0;
            for (int i = 0; i < SceneManager.sceneCount; i++)
            {
                if (!SceneManager.GetSceneAt(z).name.Equals("EmptyScene"))
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
                else
                {
                    z++;
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
            SceneManager.UnloadSceneAsync("EmptyScene");
        }
    }
}