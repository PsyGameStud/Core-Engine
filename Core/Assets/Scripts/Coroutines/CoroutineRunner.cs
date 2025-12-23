using System.Collections;
using System.Collections.Generic;
using Core.Dependency;
using UnityEngine;

namespace Services.Coroutines
{
    public class CoroutineRunner : MonoBehaviour, IDependency
    {
        private readonly HashSet<Coroutine> _coroutines = new();

        public Coroutine Run(IEnumerator routine)
        {
            Coroutine coroutine = StartCoroutine(routine);
            _coroutines.Add(coroutine);
            return coroutine;
        }

        public void Stop(Coroutine coroutine)
        {
            if(!_coroutines.Contains(coroutine)) return;
            _coroutines.Remove(coroutine);
            StopCoroutine(coroutine);
        }
    }
}