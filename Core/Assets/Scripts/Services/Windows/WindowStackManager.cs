using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UI.Base;

namespace Services.Windows
{
    public class WindowStackManager
    {
        private readonly Stack<Window> _stack = new();
        private readonly Dictionary<Window, Action> _handlers = new();

        public IReadOnlyCollection<Window> Stack => _stack;

        public void RegisterOpen(Window w)
        {
            if (!w.CanCloseFromButton) return;
            if (_stack.Contains(w)) return;

            Action handler = () => HandleClosed(w);
            _handlers[w] = handler;
            w.OnClosedWindow += handler;
            _stack.Push(w);
        }

        public void RegisterClose(Window w)
        {
            if (_stack.Count == 0) return;
            if (_stack.Peek() != w) return;

            w.Hide().Forget();
        }

        private void HandleClosed(Window w)
        {
            if (_stack.Count == 0)
            {
                TryUnsubscribe(w);
                return;
            }

            if (_stack.Peek() == w)
            {
                _stack.Pop();
            }
            else
            {
                var temp = new Stack<Window>();
                bool removed = false;
                while (_stack.Count > 0)
                {
                    var top = _stack.Pop();
                    if (top == w)
                    {
                        removed = true;
                        break;
                    }
                    temp.Push(top);
                }

                while (temp.Count > 0)
                    _stack.Push(temp.Pop());
            }

            TryUnsubscribe(w);
        }

        private void TryUnsubscribe(Window w)
        {
            if (w == null) return;
            if (_handlers.TryGetValue(w, out var handler))
            {
                w.OnClosedWindow -= handler;
                _handlers.Remove(w);
            }
        }

        public async UniTask HideTop()
        {
            if (_stack.Count == 0) return;

            var top = _stack.Peek();
            await top.Hide();
        }

        public void Clear()
        {
            foreach (var kv in _handlers)
            {
                var win = kv.Key;
                var handler = kv.Value;
                if (win != null)
                    win.OnClosedWindow -= handler;
            }
            _handlers.Clear();
            _stack.Clear();
        }
    }
}
