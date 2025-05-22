using Services.Input;
using Services.SaveSystem;
using Services.SaveSystem.SavingData;
using Services.WindowSystem;

namespace Core.EntryPointSystem
{
    public class ExampleEntryPoint : EntryPoint
    {
        private WindowService _windowService;
        private CurrencySaveData _currencyData;
        private InputService _inputService;
        
        protected override void Setup()
        {
            _windowService = Engine.GetService<WindowService>();
            _inputService = Engine.GetService<InputService>();
            _inputService.SetInput(InputType.UI);
            // example: _windowService.Show<MainMenuWindow>().Forget();
            _currencyData = Engine.GetService<SaveService>().Get<CurrencySaveData>();
        }
    }
}
