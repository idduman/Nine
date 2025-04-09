using Sirenix.OdinInspector;
using UnityEngine;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Garawell.Managers.Audio
{
    [CreateAssetMenu(fileName = "AudioManager", menuName = "Scriptlable Objects/Audio")]
    public class ScriptableAudio : ScriptableObject
    {
        [SerializeField] private AudioData[] audioData;

        public AudioData[] AudioData { get => audioData; }

#if UNITY_EDITOR
        [Button("Apply Audio")]
        public void Generate()
        {
            string filePathAndName = "Assets/Game/Scripts/Managers/Enums/AudioID.cs"; //The folder Scripts/Enums/ is expected to exist

            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum AudioID");
                streamWriter.WriteLine("{");
                for (int i = 0; i < audioData.Length; i++)
                {
                    streamWriter.WriteLine("\t" + audioData[i].AudioName + ",");
                }
                streamWriter.WriteLine("}");
            }
            AssetDatabase.Refresh();
        }
#endif
    }
}

