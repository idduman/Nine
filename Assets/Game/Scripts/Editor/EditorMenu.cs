using UnityEngine;
using UnityEditor;
using Sirenix.OdinInspector.Editor;
using Garawell.Managers;
using Garawell.Data;
using Garawell.Utility;
using Garawell.Managers.Pool;
using Garawell.Managers.Audio;

namespace Garawell.Editor
{
    public class EditorMenu : OdinMenuEditorWindow
    {
        [MenuItem("Garawell Framework/Settings", false, 30)]
        private static void OpenWindow()
        {
            EditorWindow myWindow = GetWindow<EditorMenu>("EC Framework Settings");
            myWindow.minSize = new Vector2(100, 100);
        }

        protected override OdinMenuTree BuildMenuTree()
        {
            var tree = new OdinMenuTree();
            tree.AddAssetAtPath("Editor Game Settings", "Assets/Resources/Managers/Settings.asset", typeof(EditorGameSettings));
            tree.AddAssetAtPath("Event Holder", "Assets/Game/ScriptableObjects/EventCreator.asset", typeof(EnumObject));
            tree.AddAssetAtPath("Level Manager", "Assets/Game/ScriptableObjects/Managers/LevelManager.asset", typeof(LevelManager));
            tree.AddAssetAtPath("PList Adder", "Assets/Resources/Managers/PListChanger.asset", typeof(PListData));
            tree.AddAssetAtPath("Asset Manager", "Assets/Game/ScriptableObjects/Managers/AssetManager.asset", typeof(AssetManager));
            tree.AddAssetAtPath("Currency Manager", "Assets/Game/ScriptableObjects/Managers/CurrencyManager.asset", typeof(CurrencyManager));
            tree.AddAssetAtPath("Pooling Manager", "Assets/Game/ScriptableObjects/Managers/PoolingManager.asset", typeof(ScriptablePooling));
            tree.AddAssetAtPath("Audio Manager", "Assets/Game/ScriptableObjects/Managers/AudioManager.asset", typeof(ScriptableAudio));
            return tree;
        }
    }
}

