using UnityEngine;
using System;
using UnityEngine.SceneManagement;
using Garawell.Managers.Events;
using Garawell.Managers.Scene;
using JsonFx.Json;
using Garawell.Extensions;
using Garawell.Managers.Game;
using Nine;

namespace Garawell.Managers
{
    public class GameManager : MonoBehaviour
    {
        private int tutorialLevelCount;
        private int totalLevelCount;
        private bool shuffle;
        public void Initialize()
        {
            MainManager.Instance.EventManager.Register(EventTypes.LevelRestart, LoadLevel);
            MainManager.Instance.EventManager.Register(EventTypes.LevelFinish, LoadLevel);
            MainManager.Instance.EventManager.Register(EventTypes.LevelSuccess, LevelSuccess);
            MainManager.Instance.EventManager.Register(EventTypes.LevelLoaded, LevelLoaded);
        }
        
        public void StartGame(EventArgs args)
        {
            EventRunner.LevelStart();
            MainManager.Instance.EventManager.Unregister(EventTypes.OnHoldStart, StartGame);
        }

        //Initialize PlayerController and any other level scene elements
        public void LevelLoaded(EventArgs args)
        {
            if (args is LevelLoadedArgs)
            {
                LevelLoadedArgs levelLoadedArgs = args as LevelLoadedArgs;
                if (!levelLoadedArgs.useManagerSceneLightSettings)
                {
                    SceneManager.SetActiveScene(SceneManager.GetSceneByPath(MainManager.Instance.LastLoadedScene));
                }
                tutorialLevelCount = levelLoadedArgs.tutorialLevelCount;
                totalLevelCount = levelLoadedArgs.totalLevelCount;
                shuffle = levelLoadedArgs.shuffle;
            }
            ECMonoBehaviour.Initialize();
        }

        public void LoadLevel(EventArgs args)
        {
            SceneManager.LoadScene("LevelLoaderScene", LoadSceneMode.Additive);
        }

        public void LevelSuccess(EventArgs args)
        {
            if(!NineManager.Instance.TutorialMode)
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level") + 1);
            
            int levelId = PlayerPrefs.GetInt("Level");
            LevelList levelList = JsonReader.Deserialize<LevelList>(PlayerPrefs.GetString("LevelList"));

            if (levelId == levelList.levels.Count || ((levelId - totalLevelCount) % levelList.levels.Count == 0 && levelId > 0))
            {
                levelList = CreateLevelList();
                levelList.levels.RemoveRange(0, tutorialLevelCount);

                if (shuffle)
                    levelList.levels.Shuffle();

                PlayerPrefs.SetString("LevelList", JsonWriter.Serialize(levelList));
            }
        }

        private LevelList CreateLevelList()
        {
            LevelList levelList = new LevelList();

            for (int i = 0; i < totalLevelCount; i++)
            {
                levelList.levels.Add(i);
            }
            return levelList;
        }
    }
}

