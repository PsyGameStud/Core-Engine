using System;
using UnityEngine;

namespace Core
{
    public class RuntimeBehaviour : MonoBehaviour
    {
        public event Action UpdateEvent;
        public event Action FixedUpdateEvent;
        
        private void Awake()
        {
            DontDestroyOnLoad(this);
        }
        
        private void Update()
        {
            UpdateEvent?.Invoke();
        }

        private void FixedUpdate()
        {
            FixedUpdateEvent?.Invoke();
        }

        private void OnDestroy()
        {
            Engine.Destroy(gameObject);
        }
    }
}