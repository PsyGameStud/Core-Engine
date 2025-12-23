using Services.Coroutines;
using Services.Input;
using Services.SceneLoader;
using Services.Windows;
using UnityEngine;

namespace Core.DependencyRegistrators.AdditionalRegistrators
{
    public class GlobalComponentsRegistrator : ComponentRegistrator
    {
        [Space(20)]
        [SerializeField] private InputService _inputService;
        [SerializeField] private CoroutineRunner _coroutineRunner;
        
        protected override void RegisterDependency()
        {
            RegisterMonoServiceFromPrefab<InputService>(_inputService);
            RegisterMonoServiceFromPrefab<CoroutineRunner>(_coroutineRunner);
            
            RegisterNonMonoService<WindowService>(new WindowService());
            RegisterNonMonoService<SceneLoadService>(new SceneLoadService());
        }
    }
}
