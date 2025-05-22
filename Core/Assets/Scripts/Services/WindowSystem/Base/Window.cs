using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;

namespace Services.WindowSystem.Base
{
    public abstract class Window : MonoBehaviour
    {
        [SerializeField] protected ScreenType _screenType;
        [SerializeField] protected RectTransform _root;
        [SerializeField] protected CanvasGroup _canvasGroup;
        [SerializeField] protected bool _canCloseFromButton = true;
        
        protected bool _isOpened;

        public bool CanCloseFromButton => _canCloseFromButton;
        public bool IsOpened => _isOpened;

        public event Action OnClosedWindow;

        public virtual void Setup() {}

        public virtual async UniTask Show(IWindowArgs args = null)
        {
            if (_isOpened)
                return;

            _isOpened = true;

            switch (_screenType)
            {
                case ScreenType.Popup:
                    _root.localScale = Vector3.zero;
                    gameObject.SetActive(true);
                    var tweenScale = _root.DOScale(1f, 0.3f).SetEase(Ease.OutBack);
                    await tweenScale.SetUpdate(true);
                    break;
                case ScreenType.Window:
                    await _canvasGroup.DOFade(0f, 0f).SetUpdate(true);
                    gameObject.SetActive(true);
                    var tweenFade = _canvasGroup.DOFade(1f, 0.6f);
                    await tweenFade.SetUpdate(true);
                    break;
            }
        }

        public virtual async UniTask Hide()
        {
            if (!_isOpened)
                return;

            switch (_screenType)
            {
                case ScreenType.Popup:
                    var tweenScale = _root.DOScale(0f, 0.3f).SetEase(Ease.InBack);
                    await tweenScale.SetUpdate(true);
                    break;
                case ScreenType.Window:
                    var tweenFade = _canvasGroup.DOFade(0f, 0.6f);
                    await tweenFade.SetUpdate(true);
                    break;
            }

            _isOpened = false;
            gameObject.SetActive(false);
        }

        protected void CloseWindow()
        {
            Hide().Forget();
            OnClosedWindow?.Invoke();
        }

        protected virtual void OnDestroy()
        {
            _canvasGroup.DOKill();
            _root.DOKill();
        }
    }

    public enum ScreenType
    {
        Window,
        Popup
    }
}