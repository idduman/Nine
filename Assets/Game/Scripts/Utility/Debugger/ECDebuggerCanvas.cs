using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum Anchors
{
    TopLeft, TopMid, TopRight,
    MidLeft, Middle, MidRight,
    BottomLeft, BottomMid, BottomRight
}

public struct Anchor
{
    public Vector2 min;
    public Vector2 max;
    public Vector2 pivot;
}

namespace Garawell.Utility.Debugger
{
    public class ECDebuggerCanvas : MonoBehaviour
    {

        public ECDebuggerProgress progressPrefab;
            
        [HideInInspector] public List<Text> texts = new List<Text>();
        [HideInInspector] public List<Toggle> toggles = new List<Toggle>();
        [HideInInspector] public List<ECDebuggerProgress> progressBars = new List<ECDebuggerProgress>();
        private DefaultControls.Resources UIresources = new DefaultControls.Resources();

        public void SetupResources(Sprite background, Sprite standard, Sprite checkmark, Sprite knob)
        {
            UIresources.background = background;
            UIresources.standard = standard;
            UIresources.checkmark = checkmark;
            UIresources.knob = knob;
        }

        private Anchor GetAnchor(Anchors anchor)
        {
            Anchor result = new Anchor();

            switch (anchor)
            {
                case Anchors.TopLeft:
                    result.min = new Vector2(0f, 1f);
                    result.max = new Vector2(0f, 1f);
                    result.pivot = new Vector2(0f, 1f);
                    break;
                
                case Anchors.TopMid:
                    result.min = new Vector2(0.5f, 1f);
                    result.max = new Vector2(0.5f, 1f);
                    result.pivot = new Vector2(0.5f, 1f);
                    break;
                
                case Anchors.TopRight:
                    result.min = new Vector2(1f, 1f);
                    result.max = new Vector2(1f, 1f);
                    result.pivot = new Vector2(1f, 1f);
                    break;
                
                case Anchors.MidLeft:
                    result.min = new Vector2(0f, 0.5f);
                    result.max = new Vector2(0f, 0.5f);
                    result.pivot = new Vector2(0f, 0.5f);
                    break;
                
                case Anchors.Middle:
                    result.min = new Vector2(0.5f, 0.5f);
                    result.max = new Vector2(0.5f, 0.5f);
                    result.pivot = new Vector2(0.5f, 0.5f);
                    break;
                
                case Anchors.MidRight:
                    result.min = new Vector2(1f, 0.5f);
                    result.max = new Vector2(1f, 0.5f);
                    result.pivot = new Vector2(1f, 0.5f);
                    break;
                
                case Anchors.BottomLeft:
                    result.min = new Vector2(0f, 0f);
                    result.max = new Vector2(0f, 0f);
                    result.pivot = new Vector2(0f, 0f);
                    
                    break;
                
                case Anchors.BottomMid:
                    result.min = new Vector2(0.5f, 0f);
                    result.max = new Vector2(0.5f, 0f);
                    result.pivot = new Vector2(0.5f, 0f);
                    break;
                
                case Anchors.BottomRight:
                    result.min = new Vector2(1f, 0f);
                    result.max = new Vector2(1f, 0f);
                    result.pivot = new Vector2(1f, 0f);
                    break;
            }

            return result;
        }

        private void SetRectTransformAnchor(RectTransform rectTransform, Anchors anchor)
        {
            Anchor targetAnchor = GetAnchor(anchor);
            rectTransform.anchorMin = targetAnchor.min;
            rectTransform.anchorMax = targetAnchor.max;
            rectTransform.pivot = targetAnchor.pivot;
        }

        public void DrawText(string objectName, Rect rect, Anchors anchor, string text, Color clr, int fontSize = 14, FontStyle style = FontStyle.Normal)
        {
            Text textComponent = null;
            
            textComponent = texts.Find(x => x.name == objectName);
            if (textComponent == null)
            {
                GameObject textObject = DefaultControls.CreateText(UIresources);
                textObject.name = objectName;

                textObject.transform.parent = this.transform;

                RectTransform rectTransform = textObject.GetComponent<RectTransform>();
                SetRectTransformAnchor(rectTransform, anchor);
                rectTransform.anchoredPosition = rect.position;
                rectTransform.sizeDelta = rect.size;
                
                textComponent = textObject.GetComponent<Text>();
                texts.Add(textComponent);
            }
            
            textComponent.text = text;
            textComponent.color = clr;
            textComponent.fontSize = fontSize;
            textComponent.fontStyle = style;
        }
        
        public void RemoveText(string objectName)
        {
            try
            {
                Text text = texts.Find(x => x.gameObject.name == objectName);
                texts.Remove(text);
                Destroy(text.gameObject);
            }
            catch
            {
                Debug.Log("'" + objectName + "' text not founded!");
            }
        }

        public void DrawToggle(string objectName, Rect rect, Anchors anchor, bool enabled, string label)
        {
            Toggle toggleComponent = null;

            toggleComponent = toggles.Find(x => x.name == objectName);
            if (toggleComponent == null)
            {
                GameObject toggleObject = DefaultControls.CreateToggle(UIresources);;
                toggleObject.name = objectName;

                toggleObject.transform.parent = this.transform;

                RectTransform rectTransform = toggleObject.GetComponent<RectTransform>();
                SetRectTransformAnchor(rectTransform, anchor);
                rectTransform.anchoredPosition = rect.position;
                rectTransform.sizeDelta = rect.size;
                toggleComponent = toggleObject.GetComponent<Toggle>();
                toggles.Add(toggleComponent);
            }
            
            toggleComponent.isOn = enabled;
            toggleComponent.GetComponentInChildren<Text>().text = label;
        }

        public void DrawProgress(string objectName, Rect rect, Anchors anchor, float value, float maxValue = 1f, bool showTitle = false)
        {
            ECDebuggerProgress progress = null;

            progress = progressBars.Find(x => x.name == objectName);
            if (progress == null)
            {
                ECDebuggerProgress progressObject = Instantiate(progressPrefab, transform);
                progressObject.gameObject.name = objectName;

                RectTransform rectTransform = progressObject.GetComponent<RectTransform>();
                SetRectTransformAnchor(rectTransform, anchor);
                rectTransform.anchoredPosition = rect.position;
                rectTransform.sizeDelta = rect.size;

                progress = progressObject;
                progressBars.Add(progress);
            }
            
            progress.Setup(objectName, maxValue, value, showTitle);
        }

        public void RemoveProgress(string objectName)
        {
            try
            {
                ECDebuggerProgress progress = progressBars.Find(x => x.gameObject.name == objectName);
                progressBars.Remove(progress);
                Destroy(progress.gameObject);
            }
            catch
            {
                Debug.Log("'" + objectName + "' progress not founded!");
            }
        }
    }
}
