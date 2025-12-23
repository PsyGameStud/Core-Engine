using System.Collections.Generic;
using UI.Base;
using UnityEngine;

namespace Services.Windows
{
    public class WindowContext : MonoBehaviour
    {
        private Transform _container;
        private readonly HashSet<Window> _registeredWindows = new();

        public Transform Container => _container;

        public void SetContainer(Transform container)
        {
            _container = container;
        }

        public void Register(Window window)
        {
            _registeredWindows.Add(window);
        }

        public void CleanupDestroyedWindows()
        {
            _registeredWindows.RemoveWhere(w => w == null);
        }
    }
}