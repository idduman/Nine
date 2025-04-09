using Garawell.Managers.Game;
using System;
using UnityEngine;

namespace Garawell.Managers.Scene
{
    public class LevelPrefab : ECMonoBehaviour, IDisposable
    {
        //Use this method to initialize level specific elements
        public override void OnSceneLoadComplete()
        {
            Debug.Log("Initalized Level: " + gameObject.name);
        }
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
    }
}

