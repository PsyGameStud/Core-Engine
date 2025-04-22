using System;
using System.IO;
using UnityEngine;

namespace Services.SaveSystem
{
    [Serializable]
    public abstract class SaveData
    {
        public string GetFilePath(int slot)
        {
            var dirPath = Path.Combine(Application.persistentDataPath, "Data", $"Slot_{slot}");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return Path.Combine(dirPath, GetType().Name) + ".data";
        }
        
        public string GetGlobalFilePath()
        {
            var dirPath = Path.Combine(Application.persistentDataPath, "GlobalData");
            if (!Directory.Exists(dirPath)) Directory.CreateDirectory(dirPath);
            return Path.Combine(dirPath, GetType().Name) + ".data";
        }

        public void SaveSlotData(int slot)
        {
            if (SaveService.SkipSave) return;
            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(GetFilePath(slot), json);
        }

        public void SaveGlobalData()
        {
            if (SaveService.SkipSave) return;
            var json = JsonUtility.ToJson(this, true);
            File.WriteAllText(GetGlobalFilePath(), json);
        }
    }
}