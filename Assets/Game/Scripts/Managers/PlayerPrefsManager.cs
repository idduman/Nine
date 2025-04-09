using System.Collections;
using System.Collections.Generic;
using System.IO;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Garawell.Managers
{
    [CreateAssetMenu(fileName = "PlayerPrefsManager", menuName = "Scriptlable Objects/PlayerPrefs Manager")]
    public class PlayerPrefsManager : ScriptableObject
    {

        public static PlayerPrefsManager Instance;

        public List<PrefStringData> StringPrefs;
        public List<PrefFloatData> FloatPrefs;
        public List<PrefIntData> IntPrefs;




        public void Initialize()
        {
            if (!Instance)// couldnt figured it out, how to make this singleton in framework with scriptable object. so i made a basic singleton.
                Instance = this;


            foreach (PrefStringData prefObj in StringPrefs)
            {
                if (!PlayerPrefs.HasKey(prefObj.PrefName))
                {
                    prefObj.SetString(prefObj.DefaultValue);
                }
                else
                {
                    prefObj.SetString(PlayerPrefs.GetString(prefObj.PrefName));
                }
            }


            foreach (PrefFloatData prefObj in FloatPrefs)
            {
                if (!PlayerPrefs.HasKey(prefObj.PrefName))
                {
                    prefObj.SetFloat(prefObj.DefaultValue);
                }
                else
                {
                    prefObj.SetFloat(PlayerPrefs.GetFloat(prefObj.PrefName));
                }
            }

            foreach (PrefIntData prefObj in IntPrefs)
            {
                if (!PlayerPrefs.HasKey(prefObj.PrefName))
                {
                    prefObj.SetInt(prefObj.DefaultValue);
                }
                else
                {
                    prefObj.SetInt(PlayerPrefs.GetInt(prefObj.PrefName));
                }
            }
        }

        public void SetString(PrefStringID prefID,string val)
        {
            PrefStringData psc =  StringPrefs.Find(x => x.PrefName == prefID.ToString());
            psc.SetString(val);}

        public void SetFloat(PrefFloatID prefID, float val)
        {
            PrefFloatData psc = FloatPrefs.Find(x => x.PrefName == prefID.ToString());
            psc.SetFloat(val);
        }

        public void SetInt(PrefIntID prefID, int val)
        {
            PrefIntData pid = IntPrefs.Find(x => x.PrefName == prefID.ToString());
            pid.SetInt(val);
        }



        public string GetString(PrefStringID prefID)
        {
            PrefStringData psc = StringPrefs.Find(x => x.PrefName == prefID.ToString());
            return psc.GetStringValue();
        }

        public int GetInt(PrefIntID prefID)
        {
            PrefIntData pic = IntPrefs.Find(x => x.PrefName == prefID.ToString());
            return pic.GetIntValue();
        }

        public float GetFloat(PrefFloatID prefID)
        {
            PrefFloatData pfc = FloatPrefs.Find(x => x.PrefName == prefID.ToString());
            return pfc.GetFloatValue();
        }



#if UNITY_EDITOR
        [Button("Apply Prefs")]
        public void Generate()
        {
            string filePathAndName = "Assets/Game/Scripts/Managers/Enums/PlayerPrefEnums/PrefStringID.cs"; //The folder Scripts/Enums/ is expected to exist
            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum PrefStringID");
                streamWriter.WriteLine("{");
                for (int i = 0; i < StringPrefs.Count; i++)
                {
                    streamWriter.WriteLine("\t" + StringPrefs[i].PrefName + ",");
                }
                streamWriter.WriteLine("}");
            }


            filePathAndName = "Assets/Game/Scripts/Managers/Enums/PlayerPrefEnums/PrefFloatID.cs"; //The folder Scripts/Enums/ is expected to exist
            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum PrefFloatID");
                streamWriter.WriteLine("{");
                for (int i = 0; i < FloatPrefs.Count; i++)
                {
                    streamWriter.WriteLine("\t" + FloatPrefs[i].PrefName + ",");
                }
                streamWriter.WriteLine("}");
            }


            filePathAndName = "Assets/Game/Scripts/Managers/Enums/PlayerPrefEnums/PrefIntID.cs"; //The folder Scripts/Enums/ is expected to exist
            using (StreamWriter streamWriter = new StreamWriter(filePathAndName))
            {
                streamWriter.WriteLine("public enum PrefIntID");
                streamWriter.WriteLine("{");
                for (int i = 0; i < IntPrefs.Count; i++)
                {
                    streamWriter.WriteLine("\t" + IntPrefs[i].PrefName + ",");
                }
                streamWriter.WriteLine("}");
            }


            AssetDatabase.Refresh();
        }


        [Button("Clear All Prefs")]
        public void ClearAll()
        {
            PlayerPrefs.DeleteAll();
        }


#endif
    }

}

