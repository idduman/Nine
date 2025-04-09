using Garawell.Utility;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Garawell.Managers.Events
{
    [CreateAssetMenu(fileName = "Events List", menuName = "Scriptlable Objects/Event Enum Object")]
    public class EventEnumObject : EnumObject
    {
        public EventTypes runEventType;
        [Button("Raise Event")]
        public void RaiseTestEvent()
        {
            MainManager.Instance.EventManager.InvokeEvent(runEventType);
        }
    }
}

