using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Services.SaveSystem.SavingData;
using UnityEngine;

namespace Services.SaveSystem
{
    public class SaveService : MonoBehaviour, IService
    {
        private Type _prevType;
        public static bool SkipSave = false;
        private static int _currentSlot = 0;

        private static readonly Dictionary<Type, SaveData> SAVE_LOCALS_MAP = new()
        {
            { typeof(CurrencySaveData), new CurrencySaveData() }, //Example
        };

        private static readonly Dictionary<Type, SaveData> GLOBAL_SAVE_MAP = new()
        {
            { typeof(SettingsSaveData), new SettingsSaveData() }, //Example
        };

        public void Initialize()
        {
            LoadGlobal();
            LoadLocal();

            bool hasSaveFiles = false;

            foreach (var save in SAVE_LOCALS_MAP.Values)
            {
                if (File.Exists(save.GetFilePath(_currentSlot)))
                {
                    hasSaveFiles = true;
                    break;
                }
            }

            if (!hasSaveFiles)
            {
                SaveAll();
            }
        }

        public void SetSlot(int slot)
        {
            _currentSlot = slot;
            LoadLocal();
            SaveAll();
        }

        public static T Get<T>() where T : SaveData
        {
            if (SAVE_LOCALS_MAP.TryGetValue(typeof(T), out var saveData))
            {
                return (T)saveData;
            }
            throw new Exception($"Save file of type {typeof(T).Name} not found in save map.");
        }

        public static T GetGlobal<T>() where T : SaveData
        {
            if (GLOBAL_SAVE_MAP.TryGetValue(typeof(T), out var saveData))
            {
                return (T)saveData;
            }
            throw new Exception($"Global save file of type {typeof(T).Name} not found in save map.");
        }

        private void LoadLocal()
        {
            foreach (var (type, save) in SAVE_LOCALS_MAP)
            {
                var filePath = save.GetFilePath(_currentSlot);
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, JsonUtility.ToJson(save, true));
                    continue;
                }
                
                try
                {
                    var json = File.ReadAllText(filePath);
                    JsonUtility.FromJsonOverwrite(json, save);
                }
                catch (Exception)
                {
                    Debug.LogError($"Can't parse json file for save {type.Name}");
                }
            }
        }

        private void LoadGlobal()
        {
            foreach (var (type, save) in GLOBAL_SAVE_MAP)
            {
                var filePath = save.GetGlobalFilePath();
                if (!File.Exists(filePath))
                {
                    File.WriteAllText(filePath, JsonUtility.ToJson(save, true));
                    continue;
                }
                
                try
                {
                    var json = File.ReadAllText(filePath);
                    JsonUtility.FromJsonOverwrite(json, save);
                }
                catch (Exception)
                {
                    Debug.LogError($"Can't parse json file for global save {type.Name}");
                }
            }
        }

        private void SaveAll()
        {
            if (SkipSave) return;
            
            foreach (var save in SAVE_LOCALS_MAP.Values)
            {
                save.SaveSlotData(_currentSlot);
            }
            
            foreach (var save in GLOBAL_SAVE_MAP.Values)
            {
                save.SaveGlobalData();
            }
        }
        
        public bool HasSavesInSlot(int slot)
        {
            foreach (var save in SAVE_LOCALS_MAP.Values)
            {
                var path = save.GetFilePath(slot);
                if (File.Exists(path))
                {
                    return true;
                }
            }

            return false;
        }
        
        public void ClearSaves(int slot)
        {
            if (!HasSavesInSlot(slot))
            {
                Debug.LogError($"No saves found in slot {slot}. Nothing to clear.");
                return;
            }

            var keys = new List<Type>(SAVE_LOCALS_MAP.Keys);

            foreach (var type in keys)
            {
                var newInstance = (SaveData)Activator.CreateInstance(type);
                SAVE_LOCALS_MAP[type] = newInstance;
            }

            var originalSkipState = SkipSave;
            SkipSave = false;
            foreach (var save in SAVE_LOCALS_MAP.Values)
            {
                save.SaveSlotData(slot);
            }

            SkipSave = originalSkipState;
        }

        public void Destroy()
        {
        }
    }
}