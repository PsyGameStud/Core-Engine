using System;
using System.Collections.Generic;
using System.Linq;
using Configurations.Input;
using Core;
using Cysharp.Threading.Tasks;
using Services.WindowSystem;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

namespace Services.Input
{
    public enum InputType
    {
        UI,
        Player,
    }
    
    public class InputService : IService<InputConfig>, InputControl.IPlayerActions, InputControl.IUIActions
    {
        private InputControl _gameInput;
        private EventSystem _eventSystem;
        
        private Dictionary<InputType, InputActionMap> _inputMaps = new();
        
        public EventSystem CurrentEventSystem => _eventSystem;
        public InputConfig Config { get; set; }
        public InputType CurrentInput { get; private set; }
        
        public event Action CloseUIEvent; //for esc (close top window)

        public InputService(InputConfig config)
        {
            Config = config;
        }

        public void Initialize()
        {
            _gameInput = new InputControl();
            _gameInput.UI.SetCallbacks(this);
            _gameInput.Player.SetCallbacks(this);
            
            _inputMaps = new Dictionary<InputType, InputActionMap>()
            {
                { InputType.UI , _gameInput.UI},
                { InputType.Player , _gameInput.Player},
            };
            
            var eventSystem = Engine.CreateObject("[EVENT_SYSTEM]", null, typeof(EventSystem), typeof(InputSystemUIInputModule));
            _eventSystem = eventSystem.GetComponent<EventSystem>();
            _eventSystem.GetComponent<InputSystemUIInputModule>().actionsAsset = Config.Control;
            
            CloseUIEvent += () =>
            {
                var windowService = Engine.GetService<WindowService>();
                windowService.HideTopWindowFromStack().Forget();
            };
        }
        
        public void SetInput(params InputType[] inputTypes) //Change input
        {
            foreach (var map in _inputMaps)
            {
                if (inputTypes.Contains(map.Key))
                {
                    CurrentInput = map.Key;
                    map.Value.Enable();
                }
                else
                {
                    map.Value.Disable();
                }
            }
        }
        
        public void Destroy()
        {
            CloseUIEvent = null;
        }
        
        //Player
        public void OnMove(InputAction.CallbackContext context)
        {
        }

        public void OnLook(InputAction.CallbackContext context)
        {
        }

        public void OnAttack(InputAction.CallbackContext context)
        {
        }

        public void OnInteract(InputAction.CallbackContext context)
        {
        }

        public void OnCrouch(InputAction.CallbackContext context)
        {
        }

        public void OnJump(InputAction.CallbackContext context)
        {
        }

        public void OnPrevious(InputAction.CallbackContext context)
        {
        }

        public void OnNext(InputAction.CallbackContext context)
        {
        }

        public void OnSprint(InputAction.CallbackContext context)
        {
        }
        
        //UI
        public void OnNavigate(InputAction.CallbackContext context)
        {
        }

        public void OnSubmit(InputAction.CallbackContext context)
        {
        }

        public void OnCancel(InputAction.CallbackContext context)
        {
        }

        public void OnPoint(InputAction.CallbackContext context)
        {
        }

        public void OnClick(InputAction.CallbackContext context)
        {
        }

        public void OnRightClick(InputAction.CallbackContext context)
        {
        }

        public void OnMiddleClick(InputAction.CallbackContext context)
        {
        }

        public void OnScrollWheel(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDevicePosition(InputAction.CallbackContext context)
        {
        }

        public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
        {
        }
    }
}
