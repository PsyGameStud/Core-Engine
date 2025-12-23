using UI.Base;
using UnityEngine;

namespace Services.Windows
{
    public class WindowFactory
    {
        public Window Create(Window prefab, WindowContext context)
        {
            var window = Object.Instantiate(prefab, context.Container);
            window.Setup();
            window.gameObject.SetActive(false);
            context.Register(window);
            return window;
        }
    }
}