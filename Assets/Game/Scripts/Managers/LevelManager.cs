using UnityEngine;
using Sirenix.OdinInspector;
using Garawell.Utility;
using UnityEngine.SceneManagement;
using Garawell.Managers.Scene;
using JsonFx.Json;
using Garawell.Managers.Events;

namespace Garawell.Managers
{
    [CreateAssetMenu(fileName = "LevelManager", menuName = "Scriptlable Objects/Level Manager")]
    public class LevelManager : ScriptableObject
    {
        public LevelTypes levelType;
        [ShowIf("levelType", LevelTypes.Prefab)]
        public GameObject[] levelPrefabs;
        [ShowIf("levelType", LevelTypes.Prefab)]
        public SceneReference prefabLevelScene;
        [ShowIf("levelType", LevelTypes.Scene)]
        public SceneReference managerScene;
        [ShowIf("levelType", LevelTypes.Scene)]
        public SceneReference[] scenes;
        public bool useManagerSceneLightingSettings;

        [SerializeField] private bool isRandomAfterFinish;
        [SerializeField] private int loopStartIndex;
        private int levelId = 0;
        [SerializeField] private LevelList levelList = new LevelList();
        private LevelLoader levelLoader;
       

        public int LevelId { get => levelId; }
        public LevelTypes LevelType { get => levelType; }

        public void Initialize()
        {
            if (PlayerPrefs.HasKey("Level"))
            {
                this.levelId = PlayerPrefs.GetInt("Level");
            }
            else
            {
                this.levelId = 0;
            }
            if(!PlayerPrefs.HasKey("LevelList"))
            {
                Debug.Log("Create level List");
                CreateLevelList();
            }
            else
            {
                levelList = JsonReader.Deserialize<LevelList>(PlayerPrefs.GetString("LevelList"));
                if(levelType == LevelTypes.Scene)
                {
                    if ((levelList.levels.Count != scenes.Length && levelId < scenes.Length))
                    {
                        CreateLevelList();
                    }
                }
                else if(levelType == LevelTypes.Prefab)
                {
                    if(levelList.levels.Count != levelPrefabs.Length && levelId < levelPrefabs.Length)
                    {
                        CreateLevelList();
                    }
                }
            }
            //Debug.Log("Level Count: " + levelList.levels.Count);
            //for (int i = 0; i < levelList.levels.Count; i++)
            //{
            //    Debug.Log("Level: " + levelList.levels[i]);
            //}
            LoadLevel();
        }
       
        public void LoadLevel()
        {
            levelLoader = FindObjectOfType<LevelLoader>();
            if(levelType == LevelTypes.Scene)
            {
                if(!MainManager.Instance.LastLoadedScene.Equals(""))
                {
                    SceneManager.UnloadSceneAsync(MainManager.Instance.LastLoadedScene);
                }
                MainManager.Instance.LastLoadedScene = scenes[levelList.levels[levelId % levelList.levels.Count] % scenes.Length];
                levelLoader.LoadLevel(scenes[levelList.levels[levelId % levelList.levels.Count] % scenes.Length], LoadSceneMode.Additive, () => MainManager.Instance.EventManager.InvokeEvent(EventTypes.LevelLoaded, new LevelLoadedArgs(loopStartIndex, scenes.Length, isRandomAfterFinish, true, useManagerSceneLightingSettings)));
            }
            else
            {
                if(MainManager.Instance.LastLoadedScenePrefab != null)
                {
                    SceneManager.UnloadScene(NameFromPath(prefabLevelScene));
                    Destroy(MainManager.Instance.LastLoadedScenePrefab);
                }
                levelLoader.LoadLevel(prefabLevelScene, LoadSceneMode.Additive, () =>
                {
                    LevelPrefab[] levelPrefabsOnScene = FindObjectsOfType<LevelPrefab>();
                    foreach (LevelPrefab levelPrefab in levelPrefabsOnScene)
                    {
                        DestroyImmediate(levelPrefab.gameObject);
                        levelPrefab.Dispose();
                    }
                    MainManager.Instance.LastLoadedScenePrefab = Instantiate(levelPrefabs[levelList.levels[levelId % levelList.levels.Count] % levelPrefabs.Length]);
                    MainManager.Instance.LastLoadedScene = prefabLevelScene;
                    SceneManager.MoveGameObjectToScene(MainManager.Instance.LastLoadedScenePrefab, SceneManager.GetSceneByName(NameFromPath(prefabLevelScene)));
                    MainManager.Instance.EventManager.InvokeEvent(EventTypes.LevelLoaded, new LevelLoadedArgs(loopStartIndex, levelPrefabs.Length, isRandomAfterFinish, false, useManagerSceneLightingSettings));
                });
            }  
            levelLoader.UnloadLevel();
        }

        private string NameFromPath(string buildPath)
        {
            int slash = buildPath.LastIndexOf('/');
            string name = buildPath.Substring(slash + 1);
            int dot = name.LastIndexOf('.');
            return name.Substring(0, dot);
        }

        private void CreateLevelList()
        {
            levelList.levels.Clear();
            if (levelType == LevelTypes.Scene)
            {
                for (int i = 0; i < scenes.Length; i++)
                {
                    levelList.levels.Add(i);
                }
            }
            else if (levelType == LevelTypes.Prefab)
            {
                for (int i = 0; i < levelPrefabs.Length; i++)
                {
                    levelList.levels.Add(i);
                }
            }
            PlayerPrefs.SetString("LevelList", JsonWriter.Serialize(levelList));
        }
    }

    public enum LevelTypes
    {
        None,
        Scene,
        Prefab
    }
}