#if UNITY_EDITOR
using Garawell.Managers;
using UnityEngine;

namespace Garawell.Utility.Debugger
{
    public static class ECDebugger
    {
        public static GameObject DebugCanvasPrefab => MainManager.Instance.AssetManager.GetPrefab("DebugCanvas");
        public static ECDebuggerCanvas DebuggerCanvas;

        public static Sprite Background => MainManager.Instance.AssetManager.GetSprite("Background");
        public static Sprite Standard => MainManager.Instance.AssetManager.GetSprite("Standard");
        public static Sprite CheckMark => MainManager.Instance.AssetManager.GetSprite("CheckMark");
        public static Sprite Knob => MainManager.Instance.AssetManager.GetSprite("Knob");
        
        public static void Initialize()
        {
            if (DebugCanvasPrefab == null)
            {
                Debug.Log("GW Debugger <color=#FF0000>not Initialized </color>! Check if there is DebugCanvas prefab in AssetManager.");
                return;
            }
            else if (DebuggerCanvas != null)
            {
                Debug.Log("GW Debugger <color=#FFFF00>already</color> Initialized!");
                return;
            }

            DebuggerCanvas = Object.Instantiate(DebugCanvasPrefab, Vector3.zero, Quaternion.identity).GetComponent<ECDebuggerCanvas>();
            Object.DontDestroyOnLoad(DebuggerCanvas);
            DebuggerCanvas.SetupResources(Background, Standard, CheckMark, Knob);
            Debug.Log("EC Debugger Initialized <color=#00FF00>successfully</color>!");
        }

        public static void DrawText(string name, Rect rect, Anchors anchor, string text, Color color, int fontSize = 14, FontStyle style = FontStyle.Normal)
        {
            DebuggerCanvas.DrawText(name, rect, anchor, text, color, fontSize, style);
        }
        
        public static void RemoveText(string name)
        {
            DebuggerCanvas.RemoveText(name);
        }

        public static void DrawProgress(string name, Rect rect, Anchors anchor, float value, float maxValue = 1f, bool showTitle = false)
        {
            DebuggerCanvas.DrawProgress(name, rect, anchor, value, maxValue, showTitle);
        }
        
        public static void RemoveProgress(string name)
        {
            DebuggerCanvas.RemoveProgress(name);
        }
        
        public static void DrawToggle(string name, Rect rect, Anchors anchor, bool isOn, string label)
        {
            DebuggerCanvas.DrawToggle(name, rect, anchor, isOn, label);
        }



    }    
}
#endif
