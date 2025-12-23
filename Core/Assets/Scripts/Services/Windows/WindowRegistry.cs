using System;
using System.Collections.Generic;
using System.Linq;
using UI.Base;
using Object = UnityEngine.Object;

namespace Services.Windows
{
    public class WindowRegistry
    {
        private readonly Dictionary<Type, Window> _windows = new();

        public bool TryGet<T>(out T window) where T : Window
        {
            if (_windows.TryGetValue(typeof(T), out var w))
            {
                window = (T)w;
                return true;
            }

            window = null;
            return false;
        }

        public Window GetOrCreate<T>(Func<Window> create) where T : Window
        {
            if (!_windows.TryGetValue(typeof(T), out var window) || window == null)
            {
                window = create();
                _windows[typeof(T)] = window;
            }
            return window;
        }

        public IEnumerable<Window> All() => _windows.Values.Where(w => w != null);

        public void RemoveDestroyed()
        {
            var dead = _windows
                .Where(kvp => kvp.Value == null)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var t in dead)
                _windows.Remove(t);
        }

        public void RemoveSessionWindows()
        {
            var toRemove = _windows
                .Where(kvp => kvp.Value != null && kvp.Value.CurrentContextWindowType == ContextWindowType.Session)
                .Select(kvp => kvp.Key)
                .ToList();

            foreach (var t in toRemove)
            {
                Object.Destroy(_windows[t].gameObject);
                _windows.Remove(t);
            }
        }

        public void Clear() => _windows.Clear();
    }
}