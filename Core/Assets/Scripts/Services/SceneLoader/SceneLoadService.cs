using System;
using Core.Dependency;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Services.SceneLoader
{
    public class SceneLoadService : IDependency
    {
        public event Action<SceneName, SceneName> OnLoadingStarted;
        public Action OnLoadingInProcess;
        public Action<SceneName, SceneName> OnLoadingCompleted;

        private SceneName _currentScene;
        public SceneName CurrentScene => _currentScene;

        private bool _isLoading;

        public async UniTask LoadScene(SceneName sceneName)
        {
            if (_isLoading)
                return;

            _isLoading = true;

            OnLoadingStarted?.Invoke(_currentScene, sceneName);
            await UniTask.WaitForSeconds(0.75f, true);

            OnLoadingInProcess?.Invoke();

            await SceneManager.LoadSceneAsync(sceneName.ToString());

            OnLoadingCompleted?.Invoke(_currentScene, sceneName);
            _currentScene = sceneName;

            _isLoading = false;
        }
    }

    public enum SceneName
    {
        Boot = 0,
        MainMenu = 1,
    }
}