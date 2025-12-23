using Configurations.Input;
using Configurations.Windows;
using UnityEngine;

namespace Core.DependencyRegistrators.AdditionalRegistrators
{
    public class GlobalConfigsRegistrator : ConfigRegistrator
    {
        [SerializeField] private InputConfig _inputConfig;
        [SerializeField] private WindowConfig _windowConfig;
        
        protected override void RegisterDependency()
        {
            RegisterConfig<InputConfig>(_inputConfig);
            RegisterConfig<WindowConfig>(_windowConfig);
        }
    }
}
