using System;
using Garawell.Utility;
using Garawell.Managers.Events;
using UnityEngine;

namespace Garawell.Managers
{
    public class SDKManager : MonoSingleton<SDKManager>
    {
        private bool isInitialized;
        public void Initialize()
        {
            if (!isInitialized)
            {
                MainManager.Instance.EventManager.Register(EventTypes.LevelStart, LogLevelStartEvent);
                MainManager.Instance.EventManager.Register(EventTypes.LevelFail, LogLevelFailEvent);
                MainManager.Instance.EventManager.Register(EventTypes.LevelSuccess, LogLevelSuccessEvent);
                isInitialized = true;
            }
        }

        public void LogLevelSuccessEvent(EventArgs _args)
        {
            int level = PlayerPrefs.GetInt("Level", 0) + 1;

            TinySauce.OnGameFinished(true, 0, level);
            TinySauce.TrackCustomEvent("_LevelTime:" + level + ":" + true + ":" + MainManager.Instance.MenuManager.Timer);
            Debug.LogWarning("_LevelTime:" + level + ":" + true + ":" +MainManager.Instance.MenuManager.Timer);
        }

        public void LogLevelStartEvent(EventArgs levelNumber)
        {
            int level = PlayerPrefs.GetInt("Level", 0) + 1;

            TinySauce.OnGameStarted(level);
            TinySauce.TrackCustomEvent("_LevelStart:" + level);
        }

        public void LogLevelFailEvent(EventArgs _args)
        {
            int level = PlayerPrefs.GetInt("Level", 0) + 1;

            // level fail count 
            int failCount = PlayerPrefs.GetInt("FailCount"+"level", 0);
            failCount++;
            PlayerPrefs.SetInt("FailCount"+"level", failCount);
            PlayerPrefs.Save();

            TinySauce.OnGameFinished(false, 0, level);
            TinySauce.TrackCustomEvent("_LevelTime:" + level + ":" + false + ":" + MainManager.Instance.MenuManager.Timer);
            Debug.LogWarning("LevelTime:" + level + ":" + true + ":" + MainManager.Instance.MenuManager.Timer);

            // fail event
            TinySauce.TrackCustomEvent("LevelFails:" + level + ":" + failCount);
            Debug.LogWarning("LevelFails:" + level + ":" + failCount);
        }
    }
}