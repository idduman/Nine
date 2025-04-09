using System;
using System.Collections.Generic;
using System.Linq;
using Sirenix.Utilities;
using UnityEngine;
using UnityEditor;
using Object = UnityEngine.Object;

namespace Nine
{
   public class LevelDataGenerator : EditorWindow
   {
       //public GameObject dotPrefab;
       //public GameObject hLinePrefab;
       //public GameObject vLinePrefab;
       //public GameObject squarePrefab;
       public Object _csvData;
       public string _assetName = "Level Data";
       public string _assetPath = "Assets/Game/Data";
   
       [MenuItem("Tools/Level Data Generator")]
       public static void ShowWindow()
       {
           GetWindow<LevelDataGenerator>("Level Data Generator");
       }
   
       private void OnGUI()
       {
           GUILayout.Label("Grid Settings", EditorStyles.boldLabel);
           
           _csvData = EditorGUILayout.ObjectField("CSV Data", _csvData, typeof(Object), false);
           _assetName = EditorGUILayout.TextField("Asset Name", _assetName);
           _assetPath = EditorGUILayout.TextField("Asset Path", _assetPath);
   
           if (_csvData != null && !_assetName.IsNullOrWhitespace() && !_assetPath.IsNullOrWhitespace()
           && GUILayout.Button("Generate"))
           {
               Generate();
           }
       }

       private void Generate()
       {
           var textAsset = _csvData as TextAsset;
           var text = textAsset.text;

           var levelDataManager = LevelDataManager.CreateFromString(text);
           
           AssetDatabase.CreateAsset(levelDataManager, _assetPath + "/" + _assetName + ".asset");
           AssetDatabase.SaveAssets();
           AssetDatabase.Refresh();
           EditorUtility.FocusProjectWindow();
           Selection.activeObject = levelDataManager;
       }
   }               
}