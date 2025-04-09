using System;
using Garawell.Managers;
using Sirenix.OdinInspector.Editor;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityToolbarExtender;

namespace Garawell.Editor
{
    [InitializeOnLoad]
    public class ECToolbar
    {
        public static int selectedLevel;

        public static Texture2D PackageImporterIcon;
        public static Texture2D EcSettingsIcon;
        public static Texture2D timerIcon;
        public static Texture2D prevIcon;
        public static Texture2D nextIcon;

        public static LevelManager LevelManager
        {
            get
            {
                if (levelManager == null)
                {
                    levelManager = AssetDatabase.LoadAssetAtPath<LevelManager>("Assets/Game/ScriptableObjects/Managers/LevelManager.asset");
                    return levelManager;
                }
                else
                {
                    return levelManager;
                }
            }
            set
            {
                levelManager = value;
            }
        }
        public static GUIStyle verticalLine;

        private static LevelManager levelManager;
        private static Color defaultButtonColor = new Color(1,1,1,0.75f);
        private static Color activeButtonColor = new Color(0.5f,1,0,0.75f);
        private static Color hoverButtonColor = new Color(0,0.75f,1f,1);
        private static Color settings_ButtonColor;
        private static Color packageImporter_ButtonColor;

        private static bool timerEnabled = false;
        private static EditorGameSettings editorGameSettings;
        
        static ECToolbar()
        {
            levelManager = AssetDatabase.LoadAssetAtPath<LevelManager>("Assets/Game/ScriptableObjects/Managers/LevelManager.asset");
            UpdateIcons();
            editorGameSettings = AssetDatabase.LoadAssetAtPath("Assets/Resources/Managers/Settings.asset", typeof(EditorGameSettings)) as EditorGameSettings;
            
            settings_ButtonColor = defaultButtonColor;
            packageImporter_ButtonColor = defaultButtonColor;
            
            verticalLine = new GUIStyle();
            verticalLine.normal.background = EditorGUIUtility.whiteTexture;
            verticalLine.margin = new RectOffset( 4, 4, 0, 0);
            verticalLine.fixedWidth = 1;

            timerEnabled = EditorPrefs.GetBool("GameTimer", false);
            ToolbarExtender.LeftToolbarGUI.Add(OnToolbarGUI);
        }

        static void UpdateIcons()
        {
            PackageImporterIcon = AssetManager.GetTexture2DEditor("PackageManagerIcon");
            EcSettingsIcon = AssetManager.GetTexture2DEditor("SettingsIcon");
            timerIcon = AssetManager.GetTexture2DEditor("TimerIcon");
            nextIcon = AssetManager.GetTexture2DEditor("NextIcon");
            prevIcon = AssetManager.GetTexture2DEditor("PreviousIcon");
        }

        static void OnToolbarGUI()
        {
            GUILayout.Space(4);

            if (!PackageImporterIcon || !EcSettingsIcon || !timerIcon || !nextIcon || !prevIcon)
            {
                if (GUILayout.Button("Refresh"))
                {
                    UpdateIcons();
                }
                return;
            }
            
            DrawLine();
            
            DrawButton(EcSettingsIcon, () => OpenWindow<EditorMenu>("EC Framework Settings"));
            DrawButton(PackageImporterIcon, () => OpenWindow<PackageImporter>("Package Importer"));

            DrawLine();
            DrawLine();

            if (LevelManager.levelType == LevelTypes.Scene)
            {
                DrawButton(prevIcon, () =>
                {
                    Debug.Log("Previous Level");
                });
                
                LevelPopup();
                
                DrawButton(nextIcon, () =>
                {
                    Debug.Log("Next Level");
                });
            }

            
            DrawButton(timerIcon, timerEnabled ? activeButtonColor : defaultButtonColor, hoverButtonColor, () =>
            {
                timerEnabled = !timerEnabled;
                EditorPrefs.SetBool("GameTimer", timerEnabled);
                if (timerEnabled && !editorGameSettings.ActivateDebugger)
                {
                    editorGameSettings.ActivateDebugger = true;
                }
            });
        }

        static void DrawButton(Texture texture, Action action)
        {
            GUILayout.Space(4);

            Rect rect = GUILayoutUtility.GetRect(20, 20, GUILayout.Width(20), GUILayout.Height(20));
            bool isHover = rect.Contains(Event.current.mousePosition);
            var c = GUI.color;
            GUI.color = isHover ? hoverButtonColor : defaultButtonColor;
            if (GUI.Button(rect, texture, GUIStyle.none))
            {
                action?.Invoke();
            }
            GUI.color = c;
        }
        
        static void DrawButton(Texture texture, Color defaultColor, Color hoverColor, Action action)
        {
            GUILayout.Space(4);

            Rect rect = GUILayoutUtility.GetRect(20, 20, GUILayout.Width(20), GUILayout.Height(20));
            bool isHover = rect.Contains(Event.current.mousePosition);
            var c = GUI.color;
            GUI.color = isHover ? hoverColor : defaultColor;
            if (GUI.Button(rect, texture, GUIStyle.none))
            {
                action?.Invoke();
            }
            GUI.color = c;
        }
        
        static void LevelPopup()
        {
            string[] options = new string[LevelManager.scenes.Length + 1];
            options[0] = "Default";
            for (int i = 1; i < options.Length; i++)
            {
                options[i] = LevelManager.scenes[i - 1].SceneName;
            }
            selectedLevel = EditorGUILayout.Popup(selectedLevel, options, GUILayout.Width(75));
            bool loadTargetLevel =
                options[selectedLevel] != EditorSceneManager.GetSceneAt(EditorSceneManager.sceneCount - 1).name ||
                EditorSceneManager.GetActiveScene().name != LevelManager.managerScene.SceneName;
            if (selectedLevel != 0 && loadTargetLevel)
            {
                EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo();
                EditorSceneManager.OpenScene(LevelManager.managerScene);
                EditorSceneManager.OpenScene(LevelManager.scenes[selectedLevel - 1], OpenSceneMode.Additive);
            }
            
        }

        static void DrawLine()
        {
            var c = GUI.color;
            GUI.color = new Color(1,1,1,0.2f);
            
            GUILayout.Box(GUIContent.none, verticalLine, GUILayout.Height(20));
            GUI.color = c;
        }

        static void OpenWindow<T>(string title)
        {
            if (typeof(T).BaseType == typeof(EditorWindow) || typeof(T).BaseType == typeof(OdinMenuEditorWindow))
            {
                EditorWindow window = EditorWindow.GetWindow(typeof(T), false, title);
                window.Show();
            }
        }
    }
}
