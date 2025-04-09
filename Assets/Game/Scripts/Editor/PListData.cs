using UnityEngine;

namespace Garawell.Data
{
    [CreateAssetMenu(fileName = "PListData", menuName = "Scriptlable Objects/Plist Data")]
    public class PListData : ScriptableObject
    {
        public DictionaryData<bool>[] boolList;
        public DictionaryData<int>[] integerList;
        public DictionaryData<string>[] stringList;
    }
}

