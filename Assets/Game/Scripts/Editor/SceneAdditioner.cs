using UnityEngine.SceneManagement;
using UnityEngine;

namespace Garawell.Editor
{
    public class SceneAdditioner
    {
        [RuntimeInitializeOnLoadMethod]
        public static void LoadScenes()
        {
            EditorGameSettings gameSettings = Resources.LoadAll<EditorGameSettings>("Managers")[0];
#if UNITY_EDITOR
            if (gameSettings.startFromTSPreloadScene)
            {
                SceneManager.LoadScene("TSPreloadScene");
            }
#endif
        }
    }
}
