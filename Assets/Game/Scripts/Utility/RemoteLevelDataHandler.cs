using System;
using System.Collections;
using Garawell.Managers.Events;
using UnityEngine;
using UnityEngine.Networking;

namespace Nine
{
    public class RemoteLevelDataHandler : MonoBehaviour
    {
        public static event Action<string> DataLoaded;
        
        private static readonly string RemoteLevelDataURL = 
            "https://docs.google.com/spreadsheets/d/e/2PACX-1vRNNAUSrdrEoqCWPsU0RzjcNn3MGp8HpqjdZ8rN6TmShkj3GKvRWSyI-yjksUpqvcZoAfzeKYIwYKO-/pub?output=csv";
        public string CsvData { get; private set; }
        
        private bool _loaded = false;
        
        public void Initialize()
        {
            StartCoroutine(GetDataRoutine());
        }

        public bool GetDataFromServer(out LevelDataManager levelData)
        {
            levelData = null;
            if (_loaded)
            {
                levelData = LevelDataManager.CreateFromString(CsvData);
                levelData.name = "RemoteLevelData";
                return true;
            }
            
            return false;
        }

        private IEnumerator GetDataRoutine()
        {
            UnityWebRequest req = UnityWebRequest.Get(RemoteLevelDataURL);
            yield return req.SendWebRequest();
            if (req.result == UnityWebRequest.Result.ConnectionError || req.result == UnityWebRequest.Result.ProtocolError)
            {
                Debug.LogError("Download Error: " + req.error);
                _loaded = false;
            }
            else
            {
                CsvData = req.downloadHandler.text;
                _loaded = true;
                DataLoaded?.Invoke(CsvData);
            }
            EventRunner.RemoteDataLoaded(_loaded);
        }
    }
}

