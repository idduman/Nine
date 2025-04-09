using UnityEngine;
using UnityEngine.Events;

namespace Garawell.Utility
{
    public class ECEventTrigger : MonoBehaviour
    {
        [SerializeField] private UnityEvent myEvent;

        public void Raise()
        {
            myEvent.Invoke();
        }
    }
    
}

