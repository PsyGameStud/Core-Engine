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

        private readonly Dictionary<Type, Window> _windows = new();
        private readonly List<Type> _notHideAllWindows = new() {}; //Unclosed windows
        
        public Canvas CurrentCanvas { get; private set; }
        
        public WindowService(WindowConfig configuration)
        {
            Config = configuration;
        }
        
        public void Initialize()
        {
            var canvas = Engine.CreateObject("WindowContainer", null, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            canvas.GetComponent<Canvas>().renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.GetComponent<Canvas>().overrideSorting = true;
            canvas.GetComponent<Canvas>().sortingOrder = 1;
            CurrentCanvas = canvas.GetComponent<Canvas>();

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

            _windows[typeof(T)].transform.SetAsLastSibling();
            
            foreach (var window in _windows.Values)
            {
                if (_notHideAllWindows.Contains(window.GetType()))
                {
                    window.transform.SetAsLastSibling();   
                }
            }
            
            await _windows[typeof(T)].Show(args);
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
            if (!_windows.ContainsKey(typeof(T)))
            {
                return null;
            }

            return (T)_windows[typeof(T)];
        }

        public async UniTask Hide<T>() where T : Window
        {
            if (_windows.ContainsKey(typeof(T)))
                await _windows[typeof(T)].Hide();
        }

        public void HideAll()
        {
            foreach (var window in _windows.Values)
            {
                if (!_notHideAllWindows.Contains(window.GetType()))
                    window.Hide().Forget();
            }
        }

        public void Destroy()
        {
            foreach (var window in _windows.Values)
                Object.Destroy(window);

            _windows.Clear();
        }
    }
}
