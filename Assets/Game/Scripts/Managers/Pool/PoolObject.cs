using UnityEngine;
using Sirenix.OdinInspector;
#if UNITY_EDITOR
using UnityEditor;
#endif



namespace Garawell.Managers.Pool
{
    [System.Serializable]
    public class PoolObject
    {
        [OnValueChanged("OnPrefabSet")]
        public GameObject objectPrefab;
        public string poolName;
        public int objectCount;
        public bool goBackOnDisable;

        public void OnPrefabSet()
        {
#if UNITY_EDITOR
            PoolElement poolElement;
            if(!objectPrefab.TryGetComponent<PoolElement>(out poolElement))
            {
                objectPrefab.AddComponent<PoolElement>();
                EditorUtility.SetDirty(objectPrefab);
                AssetDatabase.SaveAssets();
            }
#endif
        }
    }
}

