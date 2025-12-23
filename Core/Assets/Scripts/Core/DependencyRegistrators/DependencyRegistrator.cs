using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.DependencyRegistrators
{
    public abstract class DependencyRegistrator : MonoBehaviour
    {
        [SerializeField] private ScopeType _scopeType;
        
        protected readonly List<Type> RegistredTypes = new();

#if UNITY_EDITOR
        private void Reset()
        {
            OnScopeValueChanged();
        }

        private void OnScopeValueChanged()
        {
            string str = gameObject.name;
            int startBracket = str.IndexOf('[');
            int endBracket = str.IndexOf(']');
            
            if (startBracket != -1 && endBracket != -1)
                str = str.Remove(startBracket, endBracket - startBracket + 1);
            else
                str = " " + str;

            switch (_scopeType)
            {
                case ScopeType.Global:
                    gameObject.name = "[GLOBAL]" + str;
                    break;
                case ScopeType.Scene:
                    gameObject.name = "[SCENE]" + str;
                    break;
                case ScopeType.Session:
                    gameObject.name = "[SESSION]" + str;
                    break;
            }
        }
#endif
        
        private void Awake()
        {
            if (_scopeType == ScopeType.Global || _scopeType == ScopeType.Session)
                DontDestroyOnLoad(this);
            
            RegisterDependency();
            InitializeServices();
        }

        private void Start()
        {
            PostInitializeServices();
        }

        private void OnDestroy()
        {
            UnregisterDependency();
        }
        
        protected virtual void RegisterDependency() {}
        
        protected virtual void  UnregisterDependency() {}
        
        protected virtual void InitializeServices() {}

        protected virtual void PostInitializeServices() {}
        
        private enum ScopeType
        {
            Scene,
            Global,
            Session
        }
    }
}