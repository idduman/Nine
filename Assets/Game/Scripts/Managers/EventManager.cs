using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Garawell.Managers.Events
{
    public class EventManager
    {
        public List<EventCallback> autoUnregisterEvents = new List<EventCallback>();
        public List<EventCallback> fastEvents = new List<EventCallback>();
        public delegate void SimpleCallback(EventArgs myBool);
        private Dictionary<EventTypes, UnityAction<EventArgs>> events = new Dictionary<EventTypes, UnityAction<EventArgs>>();
        private UnityAction<EventArgs> onStationary;

        public void Initialize()
        {
            foreach (EventTypes foo in Enum.GetValues(typeof(EventTypes)))
            {
                EventHolder<EventArgs> eventHolder = new EventHolder<EventArgs>();
                events.Add(foo, eventHolder.OnEvent);
            }
            onStationary = events[EventTypes.OnStationary];
            Register(EventTypes.LevelFinish, AutoUnregister);
            Register(EventTypes.LevelRestart, AutoUnregister);
        }
        public void InvokeEvent(EventTypes eventType, EventArgs eventArgs = null)
        {
            events[eventType]?.Invoke(eventArgs);
        }

        public void Register(EventTypes eventType, UnityAction<EventArgs> callback, bool autoUnregister = false)
        {
            events[eventType] += callback;
            if (autoUnregister)
            {
                autoUnregisterEvents.Add(new EventCallback(eventType, callback));
            }
        }

        public void Unregister(EventTypes eventType, UnityAction<EventArgs> callback)
        {
            events[eventType] -= callback;
        }

        public void AutoUnregister(EventArgs args)
        {
            foreach (var item in autoUnregisterEvents)
            {
                Unregister(item.eventType, item.callback);
            }
            foreach (var item in fastEvents)
            {
                onStationary -= item.callback;
            }
            autoUnregisterEvents.Clear();
        }

        public void RegisterOnStationary(UnityAction<EventArgs> callback, bool autoUnregister)
        {
            onStationary += callback;
            if (autoUnregister)
            {
                fastEvents.Add(new EventCallback(EventTypes.OnStationary, callback));
            }
        }

        public void UnregisterOnStationary(UnityAction<EventArgs> callback)
        {
            onStationary -= callback;
            EventCallback eventCallback = fastEvents.Find(x => x.eventType == EventTypes.OnStationary);
            if (eventCallback != null)
                fastEvents.Remove(eventCallback);
        }

        public void RunOnStationary(Vector3Args args)
        {
            onStationary?.Invoke(args);
        }
    }
}

