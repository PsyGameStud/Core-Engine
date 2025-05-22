using System;
using System.Collections.Generic;
using Configurations.Windows;
using Core;
using Cysharp.Threading.Tasks;
using Services.WindowSystem.Base;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Services.WindowSystem
{
    public class WindowService : IService<WindowConfig>
    {
        public WindowConfig Config { get; set; }

        private Transform _windowContainer;
        private readonly Stack<Window> _windowStack = new();
        private readonly Dictionary<Type, Window> _windows = new();
        
        private readonly List<Type> _notHideAllWindows = new()
        {
            //any windows
        };

        public Canvas CurrentCanvas { get; private set; }

        public WindowService(WindowConfig configuration)
        {
            Config = configuration;
        }

        public void Initialize()
        {
            var canvas = Engine.CreateObject("WindowContainer", null, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvasComponent = canvas.GetComponent<Canvas>();
            canvasComponent.renderMode = RenderMode.ScreenSpaceOverlay;
            canvasComponent.overrideSorting = true;
            canvasComponent.sortingOrder = 1;
            CurrentCanvas = canvasComponent;

            var canvasScaler = canvas.GetComponent<CanvasScaler>();
            canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            canvasScaler.referenceResolution = new Vector2(1920, 1080);
            canvasScaler.matchWidthOrHeight = 0.5f;

            _windowContainer = canvas.transform;
        }

        public async UniTask Show<T>(IWindowArgs args = null) where T : Window
        {
            if (!_windows.ContainsKey(typeof(T)))
                CreateWindow<T>();

            var window = _windows[typeof(T)];
            window.transform.SetAsLastSibling();

            foreach (var win in _windows.Values)
            {
                if (_notHideAllWindows.Contains(win.GetType()))
                    win.transform.SetAsLastSibling();
            }

            RegisterOpenedWindow(window);
            await window.Show(args);
        }

        private void RegisterOpenedWindow(Window window)
        {
            if (!window.CanCloseFromButton)
                return;

            if (_windowStack.Contains(window))
                return;

            window.OnClosedWindow += RemoveFromStack;
            _windowStack.Push(window);
        }

        private void RemoveFromStack()
        {
            if (_windowStack.Count == 0)
            {
                return;
            }
            
            var window = _windowStack.Pop();
            window.OnClosedWindow -= RemoveFromStack;
        }

        public async UniTask Hide<T>() where T : Window
        {
            if (_windows.TryGetValue(typeof(T), out var window))
            {
                UnregisterWindow(window);
                await window.Hide();
            }
        }

        private void UnregisterWindow(Window window)
        {
            if (_windowStack.Count > 0 && _windowStack.Peek() == window)
            {
                RemoveFromStack();
            }
        }

        public void HideAll()
        {
            foreach (var window in _windows.Values)
            {
                if (!_notHideAllWindows.Contains(window.GetType()))
                {
                    window.Hide().Forget();
                    UnregisterWindow(window);
                }
            }

            _windowStack.Clear();
        }

        public async UniTask HideTopWindowFromStack()
        {
            if (_windowStack.Count == 0)
                return;

            var topWindow = _windowStack.Pop();
            await topWindow.Hide();
        }
        
        public void Destroy()
        {
            foreach (var window in _windows.Values)
                Object.Destroy(window);

            _windows.Clear();
            _windowStack.Clear();
        }

        private void CreateWindow<T>() where T : Window
        {
            var newWindow = Object.Instantiate(Config.GetWindow<T>(), _windowContainer);
            _windows.Add(typeof(T), newWindow);
            newWindow.gameObject.SetActive(false);
            newWindow.Setup();
        }

        public T GetWindow<T>() where T : Window
        {
            return _windows.TryGetValue(typeof(T), out var window) ? (T)window : null;
        }
    }
}