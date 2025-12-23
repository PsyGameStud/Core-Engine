using System;
using System.Collections.Generic;
using System.Linq;
using Configurations.Windows;
using Core;
using Core.Dependency;
using Cysharp.Threading.Tasks;
using Services.Input;
using UI.Base;
using UnityEngine.InputSystem;

namespace Services.Windows
{
    public class WindowService : IDependency, IInitializable, IDisposable
    {
        private InputService _inputService;
        private WindowConfig _config;

        private WindowRegistry _registry;
        private WindowStackManager _stackManager;
        private WindowContextManager _contexts;
        private WindowFactory _factory;

        private readonly List<Type> _notHideAll = new()
        {
            // typeof(LoadingWindow),
        };
        
        public WindowStackManager StackManager => _stackManager;

        public void Initialize()
        {
            _inputService = Container.Resolve<InputService>();
            _config = Container.GetConfig<WindowConfig>();

            _registry = new WindowRegistry();
            _stackManager = new WindowStackManager();
            _contexts = new WindowContextManager();
            _factory = new WindowFactory();

            _contexts.CreateGlobalContext();

            _inputService.DefaultActions.Pause.started += OnCancel;
        }

        private void OnCancel(InputAction.CallbackContext ctx)
        {
            _stackManager.HideTop().Forget();
        }

        // ------------------------------ SHOW ------------------------------
        public async UniTask Show<T>(IWindowArgs args = null) where T : Window
        {
            var prefab = _config.GetWindow<T>();

            var window = _registry.GetOrCreate<T>(() =>
            {
                var context = _contexts.Get(prefab.CurrentContextWindowType);
                return _factory.Create(prefab, context);
            });

            window.transform.SetAsLastSibling();

            _stackManager.RegisterOpen(window);
            await window.Show(args);
        }

        // ------------------------------ HIDE ------------------------------
        public async UniTask Hide<T>() where T : Window
        {
            if (_registry.TryGet<T>(out var window))
            {
                _stackManager.RegisterClose(window);
                await window.Hide();
            }
        }

        public void HideAll()
        {
            var toHide = _registry.All()
                .Where(w => !_notHideAll.Contains(w.GetType()));

            foreach (var w in toHide)
            {
                w.Hide().Forget();
                _stackManager.RegisterClose(w);
            }

            _stackManager.Clear();
            _registry.RemoveDestroyed();
        }

        // ------------------------------ SESSION RESET ------------------------------
        public void ResetSession()
        {
            _contexts.DestroySessionContext();
            _registry.RemoveSessionWindows();
        }

        // ------------------------------ CLEANUP ------------------------------
        public void Dispose()
        {
            _inputService.DefaultActions.Pause.started -= OnCancel;

            foreach (var w in _registry.All())
                if (w != null)
                    UnityEngine.Object.Destroy(w.gameObject);

            _registry.Clear();
            _stackManager.Clear();
        }
    }
}
