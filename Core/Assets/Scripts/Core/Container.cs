using System;
using System.Collections.Generic;
using System.Linq;
using Core.Dependency;
using UnityEngine;

namespace Core
{
    public static class Container
    {
        private static readonly Dictionary<Type, object> SERVICES = new();
        private static readonly Dictionary<Type, object> CONFIGS = new();

        public static bool IsProcessing { get; private set; }
        public static float Progress { get; private set; }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        private static void Initialize()
        {
            DependencyTransmitter _ = new(
                RegisterService,
                UnregisterServices,
                InitializeServices,
                PostInitializeServices,
                RegisterConfig,
                UnregisterConfig);
            
            Application.quitting += OnApplicationQuit;
        }

        public static T Resolve<T>() where T : IDependency
        {
            Type type = typeof(T);
            
            if (SERVICES.TryGetValue(type, out object service))
                return (T)service;
            
            return default;
        }

        public static bool TryResolve<T>(out T dependency) where T : IDependency
        {
            Type type = typeof(T);
            dependency = default;

            if (SERVICES.TryGetValue(type, out object service))
            {
                dependency = (T)service;
                return true;
            }
            return false;
        }

        public static T GetConfig<T>() where T : IConfig
        {
            Type type = typeof(T);

            if (CONFIGS.TryGetValue(type, out object config))
                return (T)config;
            
            return default;
        }
        
        public static bool TryGetConfig<T>(out T config) where T : IConfig
        {
            Type type = typeof(T);
            config = default;

            if (CONFIGS.TryGetValue(type, out object cfg))
            {
                config = (T)cfg;
                return true;
            }
            return false;
        }

        private static void RegisterService(Type serviceType, object serviceInstance)
        {
            SERVICES.TryAdd(serviceType, serviceInstance);
        }

        private static async void InitializeServices(Type[] types)
        {
            IsProcessing = true;
            Progress = 0;
            int serviceIndex = 0;
            List<object> intances = new();
            
            foreach (Type type in types)
            {
                if (SERVICES.TryGetValue(type, out object service)) 
                    intances.Add(service);
            }

            foreach (object instance in intances.Distinct())
            {
                if (instance is IInitializable initializeble)
                    initializeble.Initialize();
                if (instance is IAsyncInitializable asyncInitializable)
                    await asyncInitializable.Initialize();

                Progress = (float)++serviceIndex / intances.Count;
            }

            intances.Clear();
            IsProcessing = false;
        }
        
        private static async void PostInitializeServices(Type[] types)
        {
            List<object> intances = new();
            
            foreach (Type type in types)
            {
                if (SERVICES.TryGetValue(type, out object service)) 
                    intances.Add(service);
            }

            foreach (object instance in intances.Distinct())
            {
                if (instance is IPostInitializable initializeble)
                    initializeble.PostInitialize();
                if (instance is IAsyncPostInitializable asyncInitializable)
                    await asyncInitializable.PostInitialize();
            }

            intances.Clear();
        }

        private static async void UnregisterServices(Type[] types)
        {
            IsProcessing = true;
            List<object> intances = new();
            
            foreach (Type type in types)
            {
                if (!SERVICES.TryGetValue(type, out object service)) 
                    continue;
                
                intances.Add(service);
                SERVICES.Remove(type);
            }

            foreach (object instance in intances.Distinct())
            {
                if (instance is IDisposable disposable)
                    disposable.Dispose();
                if (instance is IAsyncDisposable asyncDisposable)
                    await asyncDisposable.DisposeAsync();
            }

            intances.Clear();
            IsProcessing = false;
        }
        
        private static void RegisterConfig(Type configType, object configInstance)
        {
            CONFIGS.TryAdd(configType, configInstance);
        }

        private static void UnregisterConfig(Type configType)
        {
            if (CONFIGS.ContainsKey(configType))
                CONFIGS.Remove(configType);
        }
        
        private static void OnApplicationQuit()
        {
            Shutdown();
        }
        
        private static void Shutdown()
        {
            List<object> instances = SERVICES.Values.Distinct().ToList();

            foreach (object instance in instances)
            {
                if (instance is IDisposable disposable)
                {
                    try
                    {
                        disposable.Dispose();
                    }
                    catch (Exception ex)
                    {
                        Debug.LogWarning($"Error disposing {instance.GetType().Name}: {ex}");
                    }
                }
            }

            SERVICES.Clear();
            CONFIGS.Clear();

            Debug.Log("Container shutdown complete.");
        }
    }
}
