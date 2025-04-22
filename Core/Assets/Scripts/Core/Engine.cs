using System;
using System.Collections.Generic;
using Configurations.Input;
using Configurations.Windows;
using Services.EventBus;
using Services.Input;
using Services.SaveSystem;
using Services.WindowSystem;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core
{
    public static class Engine
    {
        private static Dictionary<Type, IService> _services = new Dictionary<Type, IService>();
        public static RuntimeBehaviour Behaviour { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize()
        {
            Behaviour = new GameObject("Runtime", typeof(RuntimeBehaviour)).GetComponent<RuntimeBehaviour>();

            Application.quitting += OnApplicationQuit;

            AddService(InstantiateServiceOnEmptyObject<SaveService>(true, Behaviour.transform));
            
            AddService(new InputService(GetConfiguration<InputConfig>()));
            AddService(new WindowService(GetConfiguration<WindowConfig>()));
            AddService(new EventBusService());
        }

        public static T GetConfiguration<T>() where T : Config
        {
            return ResourceLoader.GetConfiguration<T>();
        }

        public static void AddService<T>(T service) where T : IService
        {
            if (_services.ContainsKey(typeof(T)))
                return;
            
            service.Initialize();
            _services.Add(typeof(T), service);
        }

        public static bool HasService<T>() where T : IService => _services.ContainsKey(typeof(T));

        public static void RemoveService<T>() where T : IService
        {
            if (!_services.ContainsKey(typeof(T)))
                throw new Exception($"Service {typeof(T)} is not registered.");

            if (_services.TryGetValue(typeof(T), out var service))
                service.Destroy();

            _services.Remove(typeof(T));
        }

        public static T GetService<T>() where T : IService
        {
            if (!_services.ContainsKey(typeof(T)))
                throw new Exception("Service doesn't exist.");

            return (T)_services[typeof(T)];
        }

        private static void Shutdown()
        {
            foreach (var service in _services.Values)
            {
                service.Destroy();
            }

            _services.Clear();
            Application.quitting -= OnApplicationQuit;

            if (Behaviour != null)
            {
                Object.Destroy(Behaviour.gameObject);
                Behaviour = null;
            }

            Debug.Log("Engine has been shut down and services have been cleaned up.");
        }

        private static void OnApplicationQuit()
        {
            Shutdown();
        }

        public static T Instantiate<T>(T prefab, Vector3 position, Quaternion rotation, Transform parent = null) where T : Object
        {
            if (!Behaviour)
                throw new Exception("Behaviour doesn't exist.");

            return Object.Instantiate(prefab, position, rotation, parent ? parent : Behaviour.transform);
        }

        public static T Instantiate<T>(T prefab, Transform parent = null) where T : Object
        {
            if (!Behaviour)
                throw new Exception("Behaviour doesn't exist.");

            return Object.Instantiate(prefab, parent ? parent : Behaviour.transform);
        }

        private static T InstantiateServiceOnEmptyObject<T>(bool dontDestroy = true, Transform parent = null) where T : MonoBehaviour
        {
            var serviceObject = CreateObject($"{typeof(T).Name}", parent);
            var service = serviceObject.AddComponent(typeof(T));

            if (dontDestroy)
            {
                Object.DontDestroyOnLoad(serviceObject);
            }

            return service as T;
        }

        public static GameObject CreateObject(string name, Transform parent = null, params Type[] components)
        {
            if (!Behaviour)
                throw new Exception("Behaviour doesn't exist.");

            var gameObject = new GameObject(name ?? "New object", components);
            gameObject.transform.SetParent(parent ? parent : Behaviour.transform);

            return gameObject;
        }

        public static void Destroy(GameObject gameObject, float time = 0)
        {
            Object.Destroy(gameObject, time);
        }
    }
}