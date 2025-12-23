using UI.Base;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Services.Windows
{
    public class WindowContextManager
    {
        private WindowContext _global;
        private WindowContext _session;
        private WindowContext _scene;

        // ------------------------------ GLOBAL ------------------------------
        public void CreateGlobalContext()
        {
            if (_global != null) return;

            _global = Create(
                name: "[GLOBAL_CONTEXT]",
                sortingOrder: 2,
                persistent: true
            );
        }

        // ------------------------------ GET ------------------------------
        public WindowContext Get(ContextWindowType type)
        {
            return type switch
            {
                ContextWindowType.Global => _global,
                ContextWindowType.Session => GetOrCreateSession(),
                ContextWindowType.Scene => GetOrCreateScene(),
                _ => _global
            };
        }

        private WindowContext GetOrCreateSession()
        {
            if (_session != null) return _session;
            _session = Create("[SESSION_CONTEXT]", 1, persistent: true);
            return _session;
        }

        private WindowContext GetOrCreateScene()
        {
            if (_scene != null) return _scene;

            _scene = Create("[SCENE_CONTEXT]", 0, persistent: false);
            SceneManager.MoveGameObjectToScene(_scene.gameObject, SceneManager.GetActiveScene());

            return _scene;
        }

        // ------------------------------ CREATE ------------------------------
        private WindowContext Create(string name, int sortingOrder, bool persistent)
        {
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var canvas = go.GetComponent<Canvas>();

            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;
            canvas.sortingOrder = sortingOrder;

            var scaler = go.GetComponent<CanvasScaler>();
            scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
            scaler.referenceResolution = new Vector2(1920, 1080);
            scaler.matchWidthOrHeight = 0.5f;

            if (persistent)
                Object.DontDestroyOnLoad(go);

            var ctx = go.AddComponent<WindowContext>();
            ctx.SetContainer(go.transform);

            return ctx;
        }

        // ------------------------------ DESTROY SESSION ------------------------------
        public void DestroySessionContext()
        {
            if (_session == null) return;

            Object.Destroy(_session.gameObject);
            _session = null;
        }
    }
}
