using System;
using System.Collections.Generic;
using System.Linq;
using Configurations.Input;
using Core;
using Core.Dependency;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.UI;

namespace Services.Input
{
    public enum InputType
    {
        Default = 0,
        UI = 1,
        Player = 2,
    }

    public enum ControlSchemeType
    {
        Undefined = -1,
        KeyboardAndMouse,
        Gamepad
    }
    
    public enum InputDeviceType
    {
        None = -1,
        KeyboardAndMouse,
        Xbox,
        DualShock,
        DualSense,
        NintendoSwitch
    }

    public class InputService : MonoBehaviour, IInitializable, IDependency
    {
        private const string KEYBOARD_AND_MOUSE_SCHEME = "Keyboard&Mouse";
        private const string GAMEPAD_SCHEME = "Gamepad";
        
        public event Action ControlSchemeChanged;
        public event Action OnDeviceChanged;
        
        [SerializeField] private PlayerInput _playerInput;
        
        private Dictionary<InputType, InputActionMap> _inputMaps = new();
        private InputControl _gameInput;
        private EventSystem _eventSystem;
        private InputConfig _inputConfig;
        private InputDevice _currentDevice;
        
        private InputType[] _currentInputTypes;
        
        public EventSystem CurrentEventSystem => _eventSystem;
        public InputType CurrentInput { get; private set; }
        public InputControl.UIActions UIActions { get; private set; }
        public InputControl.PlayerActions PlayerActions { get; private set; }
        public InputControl.DefaultActions DefaultActions { get; private set; }
        
        public ControlSchemeType ControlScheme { get; private set; }
        public InputDeviceType DeviceType { get; private set; }
        public string CurrentDeviceTag { get; private set; }

        public void Initialize()
        {
            _inputConfig = Container.GetConfig<InputConfig>();
            _gameInput = new InputControl();

            UIActions = _gameInput.UI;
            PlayerActions = _gameInput.Player;
            DefaultActions = _gameInput.Default;

            PlayerActions.Enable();

            _inputMaps = new Dictionary<InputType, InputActionMap>()
            {
                { InputType.UI, UIActions },
                { InputType.Player, PlayerActions },
                { InputType.Default, DefaultActions },
            };

            var eventSystem = new GameObject("[EVENT_SYSTEM]", typeof(EventSystem), typeof(InputSystemUIInputModule));
            eventSystem.transform.parent = transform;
            _eventSystem = eventSystem.GetComponent<EventSystem>();
            _eventSystem.GetComponent<InputSystemUIInputModule>().actionsAsset = _inputConfig.Control;
            
            _playerInput.onControlsChanged += OnControlsChanged;
            _playerInput.onActionTriggered += ChangeDevice;
            OnControlsChanged(_playerInput);
        }

        private void OnDestroy()
        {
            _playerInput.onControlsChanged -= OnControlsChanged;
            _playerInput.onActionTriggered -= ChangeDevice;
        }

        private void ChangeDevice(InputAction.CallbackContext context)
        {
            if(_currentDevice == context.control.device) return;
            _currentDevice = context.control.device;
            CurrentDeviceTag = _currentDevice is Mouse or Keyboard ? _currentDevice.name : GAMEPAD_SCHEME;
            OnDeviceChanged?.Invoke();
        }

        public InputType[] GetCurrentInput()
        {
            return _currentInputTypes;
        }

        public void SetInput(params InputType[] inputTypes)
        {
            _currentInputTypes = inputTypes;
            
            foreach (var map in _inputMaps)
            {
                if (inputTypes.Contains(map.Key))
                {
                    CurrentInput = map.Key;
                    map.Value.Enable();
                    continue;
                }

                map.Value.Disable();
            }
        }

        private void OnControlsChanged(PlayerInput input)
        {
            ControlSchemeType scheme = default;
            InputDeviceType device = default;

            switch (input.currentControlScheme)
            {
                case KEYBOARD_AND_MOUSE_SCHEME:
                    scheme = ControlSchemeType.KeyboardAndMouse;
                    device = InputDeviceType.KeyboardAndMouse;
                    ToggleCursor(true);
                    break;
                case GAMEPAD_SCHEME:
                    scheme = ControlSchemeType.Gamepad;
#if !PLATFORM_SWITCH
                    if (Gamepad.current is DualSenseGamepadHID)
                        device = InputDeviceType.DualSense;
                    else if (Gamepad.current is DualShockGamepad)
                        device = InputDeviceType.DualShock;
                    else
                        device = InputDeviceType.Xbox;
#else
                    device = InputDeviceType.Nintendo;
#endif
                    ToggleCursor(false);
                    break;
            }

            if (ControlScheme == scheme && DeviceType == device)
                return;

            ControlScheme = scheme;
            DeviceType = device;
            
            Debug.Log( $"Device changed to: {DeviceType.ToString()}");
            ControlSchemeChanged?.Invoke();
        }

        private void ToggleCursor(bool active) => Cursor.visible = active;
    }
}