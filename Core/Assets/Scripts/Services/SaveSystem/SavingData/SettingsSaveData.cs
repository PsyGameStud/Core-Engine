using System;
using UnityEngine;

namespace Services.SaveSystem.SavingData
{
    [Serializable]
    public class SettingsSaveData : SaveData
    {
        //Main
        public int CurrentLanguage = 0;
        public bool RunInBackground = false;
        public bool DebugMode = false;
        public bool ShowTooltip = true;
        
        //Graphic
        public ScreenMode ScreenMode = ScreenMode.FullScreenWindow;
        public Resolution Resolution = new () { height = 0, width = 0};
        public bool VSync;
        public FPSLock FPSLock = FPSLock.None;
        public int Quality = 2;
        
        //Camera
        public float FieldOfView;
        public float MouseSensitivity = 0.3f;
        
        //Music
        public float MainVolume = 1f;
        public float MusicVolume = 1f;
        public float AmbientVolume = 1f;
        public float SfxVolume = 1f;
    }

    public enum ScreenMode
    {
        FullScreenWindow = 1,
        Windowed = 3
    }

    public enum FPSLock
    {
        None = -1,
        _30 = 30,
        _60 = 60,
        _120 = 120,
        _144 = 144,
    }

    public static class ResolutionExtension
    {
        public static string ToCustomString(this Resolution resolution) => $"{resolution.width}x{resolution.height}";
        
        public static Resolution ParseResolution(this string resolutionString)
        {
            int width = 0, height = 0;
            string[] parts = resolutionString.Split('x');

            if (parts.Length == 2)
            {
                width = int.Parse(parts[0]);
                height = int.Parse(parts[1]);
            }

            return new Resolution()
            {
                height = height,
                width = width,
            };
        }
    }
}
