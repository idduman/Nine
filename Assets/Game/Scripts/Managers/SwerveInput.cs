using Garawell.Managers.Events;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Garawell.InputSystem
{
    public class SwerveInput : InputControl, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        public float sensitivity;
        private Vector3 lastPosition;
        private Vector3Args axisArgs = new Vector3Args();
        private Image inputArea;

        public override void Initialize()
        {
            inputArea = GetComponent<Image>();
            sensitivity /= Screen.width;
        }

        public override void SetInput(bool enabled)
        {
            inputArea.raycastTarget = enabled;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            lastPosition = eventData.position;
            axisArgs.x = 0f;
            axisArgs.y = 0f;
            EventRunner.HoldStart(axisArgs);
        }

        public void OnDrag(PointerEventData eventData)
        {
            axisArgs.x = (eventData.position.x - lastPosition.x) * sensitivity;
            axisArgs.y = (eventData.position.y - lastPosition.y) * sensitivity;
            EventRunner.Stationary(axisArgs);
            
            lastPosition = eventData.position;
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            axisArgs.x = 0f;
            axisArgs.y = 0f;
            EventRunner.HoldFinish(axisArgs);
        }
    }
}

