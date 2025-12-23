using Core;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace Services.SceneLoader
{
    public class AutoLoader : MonoBehaviour
    {
        private void Awake()
        {
            Container.Resolve<SceneLoadService>().LoadScene(SceneName.MainMenu).Forget();
        }
    }
}
