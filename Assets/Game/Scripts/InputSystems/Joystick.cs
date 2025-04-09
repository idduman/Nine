using UnityEngine.EventSystems;
using UnityEngine;
using DG.Tweening;
using Garawell.Managers.Events;
using Garawell.Utility;
using Sirenix.OdinInspector;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace Garawell.InputSystem
{
    public class Joystick : InputControl, IDragHandler, IPointerDownHandler, IPointerUpHandler
    {
        [Range(0f, 200f)]
        [TabGroup("General"), SerializeField] private float handlerRange = 75f;
        [TabGroup("General"), SerializeField] private CanvasGroup background;
        [TabGroup("General"), SerializeField] private Transform handler;
        [TabGroup("General"), SerializeField] private bool moveable;
        
        [TabGroup("Visual"), SerializeField] private bool showGraphics;
        [TabGroup("Visual"), SerializeField, ShowIf("$showGraphics")] private bool animated;
        [TabGroup("Visual"), SerializeField, ShowIf("$isAnimated")] private float showDuration = 2f;
        [TabGroup("Visual"), SerializeField, ShowIf("$isAnimated")] private Ease showEase;
        [TabGroup("Visual"), SerializeField, ShowIf("$isAnimated")] private Ease hideEase;
        [TabGroup("Visual"), SerializeField, ShowIf("$isAnimated")] private float overShoot = 1.70158f;
        private bool isAnimated => showGraphics && animated;
        
        private Vector2 startPoint = Vector2.zero;
        private Vector2 inputData = Vector2.zero;
        private Vector2 handlerPoint = Vector2.zero;
        private Vector2 backgroundPosition;
        private Vector2 rectSize;

        private RectTransform rectTransform;
        private Image inputArea;
        
        Sequence joystickSequence;

        public override void Initialize()
        {
            base.Initialize();
            
            if (!showGraphics)
                background.alpha = 0f;
            if (isAnimated)
                background.transform.localScale = Vector3.zero;

            rectTransform = GetComponent<RectTransform>();
            inputArea = GetComponent<Image>();
            
            rectSize = rectTransform.rect.size;
        }

        public override void SetInput(bool enabled)
        {
            inputArea.raycastTarget = enabled;
            if (!enabled)
                SetJoystickVisibility(false);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            startPoint = ScreenPosToCanvasPos(eventData.position);

            UpdateBackgroundPosition();
            handler.localPosition = Vector2.zero;        

            SetJoystickVisibility(true);
            
            EventRunner.HoldStart(new Vector3Args(inputData));
        }

        public void OnDrag(PointerEventData eventData)
        {
            handlerPoint = ScreenPosToCanvasPos(eventData.position);
            Vector2 direction = handlerPoint - startPoint;

            Vector2 directionClamped = Vector2.ClampMagnitude(direction, handlerRange);

            float dis = Vector2.Distance(handlerPoint, startPoint);

            if (moveable)
            {
                if (direction.magnitude > handlerRange) {
                    startPoint = handlerPoint - directionClamped;
                    UpdateBackgroundPosition();
                }
            }
            
            handler.localPosition = directionClamped;
            
            inputData = directionClamped / handlerRange;
            EventRunner.Stationary(new Vector3Args(inputData));
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            SetJoystickVisibility(false);
            inputData = Vector2.zero;
            EventRunner.HoldFinish(new Vector3Args(inputData));
        }
        
        public void SetJoystickVisibility(bool isVisible){
            if (showGraphics)
            {
                if (animated)
                {
                    joystickSequence?.Kill();
                    joystickSequence = DOTween.Sequence();
                    if (isVisible)
                    {
                        joystickSequence
                            .Append(background.DOFade(1f, showDuration))
                            .Join(background.transform.DOScale(Vector3.one, showDuration)
                                .SetEase(showEase, overShoot))
                            .SetUpdate(true);
                    }
                    else
                    {
                        joystickSequence
                            .Append(background.DOFade(0f, showDuration))
                            .Join(background.transform.DOScale(Vector3.zero, showDuration)
                                .SetEase(hideEase, overShoot))
                            .SetUpdate(true);
                    }
                }
                else
                {
                    background.gameObject.SetActive(isVisible);
                }
            }
        }

        private void UpdateBackgroundPosition()
        {
#if UNITY_EDITOR
            // Sometimes we change the game panel screen size in Unity editor. When we change the screen size, the rect size of the canvas also changes.
            rectSize = rectTransform.rect.size;
#endif
            backgroundPosition.x = startPoint.x - rectSize.x * 0.5f;
            backgroundPosition.y = startPoint.y - rectSize.y * 0.5f;
            background.transform.localPosition = backgroundPosition;
        }
        
        private Vector2 ScreenPosToCanvasPos(Vector2 position)
        {
            position.x = ECMathf.Remap(position.x, 0, Screen.width, 0, rectTransform.rect.size.x);
            position.y = ECMathf.Remap(position.y, 0, Screen.height, 0, rectTransform.rect.size.y);
            return position;
        }
    }
}
