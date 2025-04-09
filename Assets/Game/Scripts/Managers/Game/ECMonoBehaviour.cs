using System;
using System.Collections.Generic;
using UnityEngine;

namespace Garawell.Managers.Game
{
    public abstract class ECMonoBehaviour : MonoBehaviour
    {
        public int priorityLevel;

        private static ECMonoBehaviour[] SceneMonos;
        private static bool LoggingOn = false;
        
        public abstract void OnSceneLoadComplete();

        public static void Initialize()
        {
            SceneMonos = GameObject.FindObjectsOfType<ECMonoBehaviour>(true);

            Array.Sort(SceneMonos, new ECMonoComparetor());

            for (int i = 0; i < SceneMonos.Length; i++)
            {
                if (LoggingOn)
                    Debug.Log("Initializing :... " + SceneMonos[i]+ "-Priority:" + SceneMonos[i].priorityLevel);
                SceneMonos[i].OnSceneLoadComplete();
            }
        }
        public static void setLog(bool logOn)
        {
            LoggingOn = logOn;
        }
    }
    public class ECMonoComparetor : IComparer<ECMonoBehaviour>
    {
        public int Compare(ECMonoBehaviour x, ECMonoBehaviour y)
        {
            if (x.priorityLevel > y.priorityLevel)
                return 1;
            else if (x.priorityLevel < y.priorityLevel)
                return -1;
            else
                return 0;
        }
    }
}