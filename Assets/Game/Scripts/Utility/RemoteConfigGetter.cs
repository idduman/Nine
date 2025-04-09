using System;
using Garawell.Managers;
using UnityEngine;

public class RemoteConfigGetter : MonoBehaviour
{

    [SerializeField] private ScriptableVariable[] variables;
    
    public void Initialize()
    {
        MainManager.Instance.EventManager.Register(EventTypes.LevelLoaded, OnLevelLoaded);
        GetVariablesFromRemote();
    }

    private void OnLevelLoaded(EventArgs args)
    {
        GetVariablesFromRemote();
    }

    private void GetVariablesFromRemote()
    {
        for (int i = 0; i < variables.Length; i++)
        {
            variables[i].GetFromRemote();
        }
    }
}
