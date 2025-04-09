using UnityEngine;
using UnityEditor;
using System.IO;

namespace Garawell.Editor
{
    public class EnumGenerator : MonoBehaviour
    {
        public static void Generate(string eName, string[] elements)
        {
            string filePathAndName = "Assets/Game/Scripts/Managers/Enums/" + eName + ".cs"; //The folder Scripts/Enums/ is expected to exist

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum " + eName);
                streamWriter.WriteLine("{");
                for (int i = 0; i < elements.Length; i++)
                {
                    streamWriter.WriteLine("\t" + elements[i] + ",");
                }
                streamWriter.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }
    }
}

