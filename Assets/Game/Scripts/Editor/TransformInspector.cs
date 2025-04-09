//using System;
//using UnityEditor;
//using UnityEngine;

//namespace EC.Editor
//{
//    [CustomEditor(typeof(Transform))]
//    public class TransformInspector : UnityEditor.Editor
//    {
//        private Texture2D texture;
//        private string proportioningModeText = "AP";
//        private string angleUpdateModeText = "UA";
//        private bool inProportioningMode;
//        private bool inAngleUpdateMode;
//        private Vector3 lastScale;
//        private Vector3 lastEulerAngles;
//        private Vector3 lastInspectorAngle;
//        private bool menuSet;
//        private UnityEngine.Object lastActiveObject;
//        GUIStyle style;
//        [InitializeOnLoadMethod]
//        private void Awake()
//        {
//            texture = Resources.Load("EC", typeof(Texture2D)) as Texture2D;
//            lastScale = ((Transform)target).position;
//        }
       
//        public override void OnInspectorGUI()
//        {
//            Transform t = (Transform) target;
//            if (lastActiveObject != Selection.activeObject)
//            {
//                EditorGUILayout.Vector3Field("Rotation", t.localEulerAngles);
//                lastActiveObject = Selection.activeObject;
//                lastEulerAngles = t.localEulerAngles;
//            }

//            EditorGUI.indentLevel = 0;
//            GUI.color = Color.white;
//            style = new GUIStyle(GUI.skin.label);
//            style.fontSize = 15;
//            style.padding.left = 10;
//            GUILayout.BeginHorizontal();
//            EditorGUI.DrawPreviewTexture(new Rect(0, 0, 25, 25), texture);
//            EditorGUILayout.LabelField("EC FRAMEWORK", style);
//            EditorGUILayout.Space(25);
//            GUILayout.EndHorizontal();
//            GUI.color = Color.white;
//            Vector3 eulerAngles = Vector3.zero;
            
//            Vector3 position = EditorGUILayout.Vector3Field("Position", t.localPosition);

//            GUILayout.BeginHorizontal();
            
//            if (inAngleUpdateMode)
//            {
//                eulerAngles = EditorGUILayout.Vector3Field("Rotation", lastEulerAngles);
                
//            }
//            GUI.color = inAngleUpdateMode ? Color.red : Color.white;
//            if (GUILayout.Button(angleUpdateModeText, GUILayout.Width(30), GUILayout.Height(25)))
//            {
//                inAngleUpdateMode = !inAngleUpdateMode;
//                angleUpdateModeText = inAngleUpdateMode ? "X" : "AU";
//                lastEulerAngles = t.eulerAngles;
//            }
//            GUI.color = Color.white;
//            GUILayout.EndHorizontal();

//            GUILayout.BeginHorizontal();
//            Vector3 scale = EditorGUILayout.Vector3Field("Scale", t.localScale);
//            GUI.color = inProportioningMode ? Color.red : Color.white;
//            if (GUILayout.Button(proportioningModeText, GUILayout.Width(30), GUILayout.Height(25)))
//            {
//                inProportioningMode = !inProportioningMode;
//                proportioningModeText = inProportioningMode ? "X" : "AP";
//                lastScale = t.localScale;
//            }
           
//            GUILayout.EndHorizontal();
//            GUI.color = Color.white;
//            if (GUI.changed)
//            {
//                Undo.RegisterCompleteObjectUndo(t, "Transform Change");
//                t.localPosition = FixIfNaN(position);
//                t.localEulerAngles = FixIfNaN(eulerAngles);
//                t.localScale = FixIfNaN(scale);
//            }
//            GUILayout.BeginHorizontal();
        
//            GUI.color = new Color(0.5f, 0.8f, 1f);
//            if (GUILayout.Button("Reset Position"))
//            {
//                Undo.RegisterCompleteObjectUndo(t, "Reset Position " + t.name);
//                t.transform.position = Vector3.zero;
//            }
        
//            if (GUILayout.Button("Reset Rotation"))
//            {
//                Undo.RegisterCompleteObjectUndo(t, "Reset Rotation " + t.name);
//                t.transform.rotation = Quaternion.identity;
//            }
        
//            if (GUILayout.Button("Reset Scale"))
//            {
//                Undo.RegisterCompleteObjectUndo(t, "Reset Scale " + t.name);
//                t.transform.localScale = Vector3.one;
//            }

//            if(inProportioningMode)
//            {
//                if(Mathf.Abs(lastScale.x - t.localScale.x) > .001f)
//                {
//                    Vector3 newPos = t.localScale;
//                    newPos.y = t.localScale.x;
//                    newPos.z = t.localScale.x;
//                    t.transform.localScale = newPos;
//                }
//                if (Mathf.Abs(lastScale.y - t.localScale.y) > .001f)
//                {
//                    Vector3 newPos = t.localScale;
//                    newPos.x = t.localScale.y;
//                    newPos.z = t.localScale.y;
//                    t.transform.localScale = newPos;
//                }
//                if (Mathf.Abs(lastScale.z - t.localScale.z) > .001f)
//                {
//                    Vector3 newPos = t.localScale;
//                    newPos.y = t.localScale.z;
//                    newPos.x = t.localScale.z;
//                    t.transform.localScale = newPos;
//                }
//                lastScale = t.localScale;
//            }
//            GUILayout.EndHorizontal();
//            GUILayout.BeginHorizontal();
        
//            GUI.color = new Color(0.74f, 1f, 0.4f);
//            if (GUILayout.Button("Enable Colliders"))
//            {
//                Collider[] colliders = t.transform.GetComponentsInChildren<Collider>();
//                for (int i = 0; i < colliders.Length; i++) colliders[i].enabled = true;
//            }
        
//            GUI.color = new Color(1f, 0.67f, 0.4f);
//            if (GUILayout.Button("Disable Colliders"))
//            {
//                Collider[] colliders = t.transform.GetComponentsInChildren<Collider>();
//                for (int i = 0; i < colliders.Length; i++) colliders[i].enabled = false;
//            }
        
//            GUILayout.EndHorizontal();
//            GUI.color = Color.white;
//        }

//        private Rect Rect(int v1, int v2, int v3, double v4)
//        {
//            throw new NotImplementedException();
//        }

//        private Vector3 FixIfNaN(Vector3 v)
//        {
//            if (float.IsNaN(v.x)) v.x = 0;
//            if (float.IsNaN(v.y)) v.y = 0;
//            if (float.IsNaN(v.z)) v.z = 0;
//            return v;
//        }
//    }
//}