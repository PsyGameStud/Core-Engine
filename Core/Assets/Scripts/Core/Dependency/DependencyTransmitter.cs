using System;
using UnityEngine;

namespace Core.Dependency
{
    public class DependencyTransmitter
    {
        public static DependencyTransmitter Instance {get; private set;}
        
        private readonly Action<Type, object> _registerServiceCallback;
        private readonly Action<Type[]> _unregisterServiceCallback;
        private readonly Action<Type[]> _initializeServiceCallback;
        private readonly Action<Type[]> _postInitializeServiceCallback;
        private readonly Action<Type, object> _registerConfigCallback;
        private readonly Action<Type> _unregisterConfigCallback;
        
        public DependencyTransmitter(
            Action<Type, object> registerServiceCallback, 
            Action<Type[]> unregisterServiceCallback,
            Action<Type[]> initializeServiceCallback,
            Action<Type[]> postInitializeServiceCallback,
            Action<Type, object> registerConfigCallback,
            Action<Type> unregisterConfigCallback)
        {
            Instance = this;
            
            _registerServiceCallback = registerServiceCallback;
            _unregisterServiceCallback = unregisterServiceCallback;
            _initializeServiceCallback = initializeServiceCallback;
            _postInitializeServiceCallback = postInitializeServiceCallback;
            _registerConfigCallback = registerConfigCallback;
            _unregisterConfigCallback = unregisterConfigCallback;
        }

        public void RegisterService(object serviceInstance, params Type[] contractTypes)
        {
            if (contractTypes is null)
            {
                Debug.LogError("Type of service not specified");
                return;
            }
            
            foreach (Type contractType in contractTypes)
                _registerServiceCallback?.Invoke(contractType, serviceInstance);
        }

        public void InitializeService(Type[] types)
        {
            _initializeServiceCallback?.Invoke(types);
        }
        
        public void PostInitializeService(Type[] types)
        {
            _postInitializeServiceCallback?.Invoke(types);
        }

        public void UnregisterService(Type[] types)
        {
            _unregisterServiceCallback?.Invoke(types);
        }

        public void RegisterConfig(Type type, object config)
        {
            _registerConfigCallback?.Invoke(type, config);
        }

        public void UnregisterConfig(Type type)
        {
            _unregisterConfigCallback?.Invoke(type);
        }
    }
}