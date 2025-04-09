using System.Collections;
using UnityEngine;
using Sirenix.OdinInspector;
using System;

namespace Garawell.Managers.Menu
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PanelManager : MonoBehaviour
    {
        protected CanvasGroup canvasGroup;
        [MinValue(0.01f)]
        public float appearTime;
        [MinValue(0.01f)]
        public float disappearTime;

        private Coroutine appearDisappearCoroutine;

        public virtual void Initialize()
        {
            canvasGroup = GetComponent<CanvasGroup>();
        }
        public virtual void Appear(EventArgs eventArgs = null)
        {
            StopAppearDisappearCoroutine();
            gameObject.SetActive(true);
            appearDisappearCoroutine = StartCoroutine(AppearRoutine());
        }

        public IEnumerator AppearRoutine()
        {
            while (canvasGroup.alpha < 1)
            {
                canvasGroup.alpha += Time.unscaledDeltaTime / appearTime;
                yield return null;
            }
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }

        public virtual void Disappear()
        {
            if (!gameObject.activeSelf)
                return;
            
            StopAppearDisappearCoroutine();
            appearDisappearCoroutine = StartCoroutine(DisappearRoutine());
        }

        private void StopAppearDisappearCoroutine()
        {
            if (appearDisappearCoroutine != null)
            {
                StopCoroutine(appearDisappearCoroutine);
            }
        }

        public IEnumerator DisappearRoutine()
        {
            if (canvasGroup == null)
                canvasGroup = GetComponent<CanvasGroup>();

            while (canvasGroup.alpha > 0)
            {
                canvasGroup.alpha -= Time.unscaledDeltaTime / disappearTime;
                yield return null;
            }
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            gameObject.SetActive(false);
        }
    }
}

