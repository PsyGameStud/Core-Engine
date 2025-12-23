using UI.Base;
using UnityEngine;
using UnityEngine.UI;

namespace UI.MainMenu
{
    public class MainMenuWindow : Window
    {
        [SerializeField] private Button _fastStartButton;
        [SerializeField] private Button _settingsButton;
        [SerializeField] private Button _exitButton;


        public override void Setup()
        {
            _fastStartButton.onClick.AddListener(StartGame);
            _settingsButton.onClick.AddListener(OpenSettings);
            _exitButton.onClick.AddListener(ExitGame);
        }

        private void StartGame()
        {
        }

        private void OpenSettings()
        {
            
        }

        private void ExitGame()
        {
            
        }
    }
}
