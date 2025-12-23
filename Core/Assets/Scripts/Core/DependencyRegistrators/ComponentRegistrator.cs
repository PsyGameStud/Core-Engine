using System;
using Core.Dependency;
using UnityEngine;

namespace Core.DependencyRegistrators
{
    [DefaultExecutionOrder(-4000)]
    public abstract class ComponentRegistrator : DependencyRegistrator
    {
        protected void RegisterMonoServiceFromInstance<T>(object instance, bool reparent = false) where T : IDependency
        {
            RegisterMonoServiceFromInstance(instance, reparent, typeof(T));
        }
        
        protected void RegisterMonoServiceFromInstance(object instance, bool reparent = false, params Type[] contractTypes)
        {
            DependencyTransmitter.Instance.RegisterService(instance, contractTypes);
            UpdateRegistredTypes(contractTypes);
            if (reparent)
            {
                if (instance is MonoBehaviour monoBehaviour && monoBehaviour.transform.parent != transform)
                    monoBehaviour.transform.SetParent(transform);
            }
        }

        protected void RegisterMonoServiceFromPrefab<T>(Component prefab) where T : IDependency
        {
            RegisterMonoServiceFromPrefab(prefab, typeof(T));
        }
        
        protected void RegisterMonoServiceFromPrefab(Component prefab, params Type[] contractTypes)
        {
            Component component = Instantiate(prefab, transform);
            DependencyTransmitter.Instance.RegisterService(component, contractTypes);
            UpdateRegistredTypes(contractTypes);
        }

        protected void RegisterNonMonoService<T>(object instance) where T : IDependency
        {
            RegisterNonMonoService(instance, typeof(T));
        }
        
        protected void RegisterNonMonoService(object instance, params Type[] contractTypes)
        {
            DependencyTransmitter.Instance.RegisterService(instance, contractTypes);
            UpdateRegistredTypes(contractTypes);
        }

        protected sealed override void InitializeServices()
        {
            DependencyTransmitter.Instance.InitializeService(RegistredTypes.ToArray());
        }

        protected override void PostInitializeServices()
        {
            DependencyTransmitter.Instance.PostInitializeService(RegistredTypes.ToArray());
        }

        protected sealed override void UnregisterDependency()
        {
            DependencyTransmitter.Instance.UnregisterService(RegistredTypes.ToArray());
        }

        private void UpdateRegistredTypes(Type[] types)
        {
            foreach (Type type in types)
                RegistredTypes.Add(type);
        }
    }
}