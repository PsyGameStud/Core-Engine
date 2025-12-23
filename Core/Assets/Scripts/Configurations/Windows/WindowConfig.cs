using System;
using System.Collections.Generic;
using Core;
using UI.Base;
using UnityEngine;

namespace Configurations.Windows
{
    [CreateAssetMenu(menuName = "Create Config / Window Config", fileName = "WindowConfig")]
    public class WindowConfig : Config
    {
        [SerializeField] private List<Window> _windows;

        public T GetWindow<T>() where T : Window
        {
            foreach (var window in _windows)
            {
                if (window is T result)
                    return result;
            }

            throw new Exception("Window type not found");
        }
    }
}
