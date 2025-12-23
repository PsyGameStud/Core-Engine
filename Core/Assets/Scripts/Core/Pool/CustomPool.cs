using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

namespace Core.Pool
{
    public class CustomPool<T> where T : MonoBehaviour
    {
        private readonly Queue<T> _objects = new Queue<T>();

        private readonly T _prefab;
        private readonly Transform _parent;

        public CustomPool(T prefab, int prewarmObjects, Transform parent = null)
        {
            _prefab = prefab;
            _parent = parent;

            for (int i = 0; i < prewarmObjects; i++)
                Create();
        }

        public T Get(Action<T> beforeActivate = null)
        {
            while (_objects.Count > 0 && _objects.Peek() == null)
                _objects.Dequeue();

            if (_objects.Count == 0)
            {
                Create();
            }

            T obj = _objects.Dequeue();

            beforeActivate?.Invoke(obj);
            obj.gameObject.SetActive(true);

            return obj;
        }

        public void Release(T obj)
        {
            _objects.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }

        private void Create()
        {
            var obj = Object.Instantiate(_prefab, _parent);
            _objects.Enqueue(obj);
            obj.gameObject.SetActive(false);
        }
        
        public void Clear()
        {
            foreach (var obj in _objects)
            {
                if (obj != null)
                    Object.Destroy(obj.gameObject);
            }

            _objects.Clear();
        }
    }
}
