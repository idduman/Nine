using System;
using Garawell.Managers;
using Garawell.Utility.Debugger;
using UniRx;
using UnityEngine;

public class UtilityManager
{
#if UNITY_EDITOR
    public static bool timerEnabled => UnityEditor.EditorPrefs.GetBool("GameTimer");
    private IDisposable timer;
    private float startTime;
#endif
    
    public void Initialize()
    {
#if UNITY_EDITOR
        RegisterEvents();
#endif
    }
    
#if UNITY_EDITOR
    private void RegisterEvents()
    {
        MainManager.Instance.EventManager.Register(EventTypes.LevelStart, OnLevelStart);
        MainManager.Instance.EventManager.Register(EventTypes.LevelFinish, OnLevelFinish);
        MainManager.Instance.EventManager.Register(EventTypes.LevelLoaded, OnLevelLoaded);
    }
    
    private void OnLevelStart(EventArgs arg0)
    {
        if (timerEnabled)
        {
            startTime = Time.unscaledTime;
            timer = Observable.EveryUpdate()
                .TakeUntilDisable(MainManager.Instance)
                .UniSubscribe(_ =>
                {
                    float time = Time.unscaledTime - startTime;
                    ECDebugger.DrawText("Timer", new Rect(0,-20,300,75), Anchors.TopMid, "PlayTime : " + (int)time, Color.black, 35, FontStyle.Bold);
                });
        }
    }
    
    private void OnLevelFinish(EventArgs arg0)
    {
        if (timerEnabled)
        {
            timer?.Dispose();
        }
    }

    private void OnLevelLoaded(EventArgs arg0)
    {
        if (timerEnabled)
        {
            timer?.Dispose();
            ECDebugger.RemoveText("Timer");
        }
    }
    #endif
}
