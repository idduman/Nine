using UnityEngine;
using UnityEditor;
using UnityEditor.Callbacks;
#if UNITY_IOS
using UnityEditor.iOS.Xcode;
#endif
using System.IO;
using Garawell.Data;

namespace Garawell.Editor
{
    
    public class PListChanger
    {
        [PostProcessBuild]
        public static void ChangeXcodePlist(BuildTarget buildTarget, string pathToBuiltProject)
        {
#if UNITY_IOS
            if (buildTarget == BuildTarget.iOS)
            {
                 PListData[] pListData = Resources.LoadAll<PListData>("Managers");
                
                // Get plist
                 string plistPath = pathToBuiltProject + "/Info.plist";
                PlistDocument plist = new PlistDocument();
                plist.ReadFromString(File.ReadAllText(plistPath));

                // Get root
                PlistElementDict rootDict = plist.root;
                foreach (var item in pListData[0].boolList)
                {
                    Debug.Log("Boolean found: " + item.key);
                    rootDict.SetBoolean(item.key, item.value);
                }
                foreach (var item in pListData[0].integerList)
                {
                    rootDict.SetInteger(item.key, item.value);
                }
                foreach (var item in pListData[0].stringList)
                {
                    rootDict.SetString(item.key, item.value);
                }
                // Write to file
                File.WriteAllText(plistPath, plist.WriteToString());
            }
#endif
        }
    }
}
