using System;
using System.Collections.Generic;
using System.Linq;
using Core;
using UnityEngine;

namespace Services.EventBus
{
    public class EventBusService : IService
    {
        private readonly Dictionary<string, List<CallbackWithPriority>> _signalCallbacks = new ();

        public void Initialize() {}
        public void Destroy() {}
        
        public void Subscribe<T>(Action<T> callback, int priority = 0)
        {
            string key = typeof(T).Name;
            if (_signalCallbacks.TryGetValue(key, out List<CallbackWithPriority> signalCallback))
            {
                signalCallback.Add(new CallbackWithPriority(priority, callback));
            }
            else
            {
                _signalCallbacks.Add(key, new List<CallbackWithPriority>() { new(priority, callback) });
            }

            _signalCallbacks[key] = _signalCallbacks[key].OrderByDescending(x => x.Priority).ToList();
        }

        public void SendSignal<T>(T signal)
        {
            string key = typeof(T).Name;
            
            if (_signalCallbacks.TryGetValue(key, out var signalCallback))
            {
                foreach (var obj in signalCallback)
                {
                    var callback = obj.Callback as Action<T>;
                    callback?.Invoke(signal);
                }
            }
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            string key = typeof(T).Name;
            if (_signalCallbacks.ContainsKey(key))
            {
                var callbackToDelete = _signalCallbacks[key].FirstOrDefault(x => x.Callback.Equals(callback));
                if (callbackToDelete != null)
                {
                    _signalCallbacks[key].Remove(callbackToDelete);
                }
            }
            else
            {
                Debug.LogErrorFormat("Trying to unsubscribe for not existing key! {0} ", key);
            }
        }
    }
}
