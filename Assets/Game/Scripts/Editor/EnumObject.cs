using UnityEngine;
using Sirenix.OdinInspector;
using Garawell.Editor;

namespace Garawell.Utility
{
    [CreateAssetMenu(fileName = "Enum List", menuName = "Scriptlable Objects/Enum Object")]
    public class EnumObject : ScriptableObject
    {
        public string enumName;
        public string[] elements;
        
        [Button("Create/Update Enum")]
        public void CreateEnum()
        {
            EnumGenerator.Generate(enumName, elements);
        }
    }
}

