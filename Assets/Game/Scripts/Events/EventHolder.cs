using UnityEngine.Events;

namespace Garawell.Managers.Events
{
    public class EventHolder<T>
    {
        public UnityAction<T> OnEvent;

        public EventHolder()
        {

        }
    }
}

