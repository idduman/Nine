using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace Voodoo.Tiny.Sauce.Internal.Ads
{
    public class TsAdAnimationBehavior : MonoBehaviour
    {
        public enum Position { Lower = -877, Higher = -717 }
        
        private Animator _animator;

        private RectTransform _rectTransform;

        private void Awake()
        {
            _rectTransform = GetComponent<RectTransform>();
        }

        public void SetTrigger(string trigger)
        {
            _animator.SetTrigger(trigger);
        }

        public void SetPosition(Position position)
        {
            _rectTransform.anchoredPosition = new Vector2(_rectTransform.anchoredPosition.x, (float)position);
        }
    }
}