using Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Configurations.Input
{
    [CreateAssetMenu(menuName = "Create Config / Input Config", fileName = "InputConfig")]
    public class InputConfig : Config
    {
        public InputActionAsset Control;
    }
}
