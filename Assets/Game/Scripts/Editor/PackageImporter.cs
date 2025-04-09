using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Compilation;
using UnityEngine;

namespace Garawell.Editor
{
    public class PackageImporter : EditorWindow
    {
        private Package[] packages;
        private bool haveScroll = false;
        private Vector2 scrollPos;

        private GUIStyle boxStyle;
        private GUIStyle lineStyle;
        private GUIStyle titleStyle;
        private GUIStyle lowTitleStyle;

        [MenuItem("Garawell Framework/Package Importer")]
        private static void ShowWindow()
        {
            var window = GetWindow<PackageImporter>();
            window.titleContent = new GUIContent("Importer");
            window.minSize = new Vector2(500, 400);
            window.maxSize = new Vector2(500, 400);
            window.Show();
        }

        private void OnEnable()
        {
            packages = Resources.LoadAll<Package>("Packages");
            haveScroll = packages.Length > 11;
            titleStyle = new GUIStyle();
            titleStyle.normal.textColor = Color.white;
            titleStyle.fontSize = 16;
            titleStyle.alignment = TextAnchor.MiddleLeft;
            titleStyle.fontStyle = FontStyle.Bold;

            lowTitleStyle = new GUIStyle();
            lowTitleStyle.normal.textColor = new Color(1, 1, 1, 0.8f);
            lowTitleStyle.fontSize = 10;
            lowTitleStyle.alignment = TextAnchor.LowerRight;
            lowTitleStyle.fontStyle = FontStyle.Italic;

            boxStyle = new GUIStyle();
            boxStyle.normal.background = MakeTex(2, 2, new Color(0f, 0f, 0f, 0.12f));

            lineStyle = new GUIStyle();
            lineStyle.normal.background = MakeTex(1, 1, new Color(1f, 1f, 1f, 0.25f));
        }

        private void OnGUI()
        {
            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label("Packages", titleStyle, GUILayout.MaxHeight(20));
            GUILayout.Label("Count : " + packages.Length, lowTitleStyle, GUILayout.MaxHeight(20));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUI.DrawTexture(new Rect(5, 25, position.width - 10, 1), lineStyle.normal.background);

            scrollPos = GUILayout.BeginScrollView(scrollPos, false, haveScroll);

            for (int i = 0; i < packages.Length; i++)
            {
                packages[i] = DrawPackage(packages[i], i);
            }
            if (GUILayout.Button("Apply", GUILayout.MaxWidth(75), GUILayout.MinHeight(20), GUILayout.MaxHeight(20)))
            {
                CompilationPipeline.RequestScriptCompilation();
                AssetDatabase.SaveAssets();
                AssetDatabase.Refresh();
            }
            GUILayout.EndScrollView();
        }

        private Package DrawPackage(Package package, int index)
        {
            GUILayout.Space(index == 0 ? 9 : 8);

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            GUI.DrawTexture(new Rect(5, 5 + index * 35, position.width - 10, 30), boxStyle.normal.background);
            GUILayout.Label(package.packageName, GUILayout.MinHeight(20), GUILayout.MaxHeight(20));

            if (!package.imported)
            {
                if (GUILayout.Button("Import", GUILayout.MaxWidth(75), GUILayout.MinHeight(20), GUILayout.MaxHeight(20)))
                {
                    package.imported = Import(package);
                    EditorUtility.SetDirty(package);
                }
            }
            else
            {
                if (GUILayout.Button("Remove", GUILayout.MaxWidth(75), GUILayout.MinHeight(20), GUILayout.MaxHeight(20)))
                {
                    if (Remove(package))
                    {
                        package.imported = false;
                        EditorUtility.SetDirty(package);
                    }
                }
            }

            GUILayout.Space(10);
            GUILayout.EndHorizontal();
            GUILayout.Space(5);

            return package;
        }

        private bool Import(Package package)
        {
            AssetDatabase.ImportPackage(package.path, false);

            if (package.dependencies != "")
            {
                List<string> defineSymbols = new List<string>();
                string[] defines = new string[0];
                PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out defines);
                defineSymbols = new List<string>(defines);

                if (!defineSymbols.Contains(package.dependencies))
                {
                    defineSymbols.Add(package.dependencies);
                    SetScriptingDefineSymbolsForAll(defineSymbols);
                    AssetDatabase.SaveAssets();
                }
            }
            return true;        
        }

        private bool Remove(Package package)
        {
            foreach (string path in package.importedPath)
            {
                AssetDatabase.DeleteAsset(path);
            }

            if (package.dependencies != "")
            {
                List<string> defineSymbols = new List<string>();
                string[] defines = new string[0];
                PlayerSettings.GetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, out defines);
                defineSymbols = new List<string>(defines);

                if (defineSymbols.Contains(package.dependencies))
                {
                    defineSymbols.Remove(package.dependencies);
                    SetScriptingDefineSymbolsForAll(defineSymbols);
                    AssetDatabase.SaveAssets();
                }
            }
            return true;
        }

        private void SetScriptingDefineSymbolsForAll(List<string> defineSymbols)
        {
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.iOS, defineSymbols.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Android, defineSymbols.ToArray());
            PlayerSettings.SetScriptingDefineSymbolsForGroup(BuildTargetGroup.Standalone, defineSymbols.ToArray());
        }

        private Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            {
                pix[i] = col;
            }
            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }
    }
}