using UnityEngine;
using UnityEngine.SceneManagement;

namespace Garawell.Utility
{
    public class SceneLoader : MonoBehaviour
    {
        public string[] sceneNames;
        void Start()
        {
            if (PlayerPrefs.HasKey("Level"))
            {
                int levelNumber = PlayerPrefs.GetInt("Level") % sceneNames.Length;
                SceneManager.LoadScene(sceneNames[levelNumber]);
            }
            else
            {
                SceneManager.LoadScene(sceneNames[0]);
            }
        }
    }
}

