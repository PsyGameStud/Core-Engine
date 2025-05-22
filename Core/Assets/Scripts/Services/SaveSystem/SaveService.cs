using System;
using System.Collections.Generic;
using System.IO;
using Core;
using Services.SaveSystem.SavingData;
using UnityEngine;

namespace Services.SaveSystem
{
    public class SaveService : IService
    {
        private readonly Dictionary<Type, SaveData> _localSaves = new()
        {
            { typeof(CurrencySaveData), new CurrencySaveData() },
        };

        private readonly Dictionary<Type, SaveData> _globalSaves = new()
        {
            { typeof(SettingsSaveData), new SettingsSaveData() },
        };

        private int _currentSlot = 0;
        public static bool SkipSave { get; private set; }

        public void Initialize()
        {
            LoadGlobal();
            LoadLocal();

            bool hasSaveFiles = false;
            foreach (var save in _localSaves.Values)
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

        public T Get<T>() where T : SaveData
        {
            if (_localSaves.TryGetValue(typeof(T), out var saveData))
            {
                return (T)saveData;
            }
            throw new Exception($"Save file of type {typeof(T).Name} not found in save map.");
        }

        public T GetGlobal<T>() where T : SaveData
        {
            if (_globalSaves.TryGetValue(typeof(T), out var saveData))
            {
                return (T)saveData;
            }
            throw new Exception($"Global save file of type {typeof(T).Name} not found in save map.");
        }

        private void LoadLocal()
        {
            foreach (var (type, save) in _localSaves)
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
            foreach (var (type, save) in _globalSaves)
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

        public void SaveAll()
        {
            if (SkipSave) return;

            foreach (var save in _localSaves.Values)
            {
                save.SaveSlotData(_currentSlot);
            }

            foreach (var save in _globalSaves.Values)
            {
                save.SaveGlobalData();
            }
        }

        public bool HasSavesInSlot(int slot)
        {
            foreach (var save in _localSaves.Values)
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

            var keys = new List<Type>(_localSaves.Keys);

            foreach (var type in keys)
            {
                var newInstance = (SaveData)Activator.CreateInstance(type);
                _localSaves[type] = newInstance;
            }

            var originalSkipState = SkipSave;
            SkipSave = false;
            foreach (var save in _localSaves.Values)
            {
                save.SaveSlotData(slot);
            }
            SkipSave = originalSkipState;
        }

        public void DeleteSlotFolder(int slot)
        {
            var path = Path.Combine(Application.persistentDataPath, $"Slot_{slot}");
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }

        public void ClearSlotFolderContents(int slot)
        {
            var folderPath = Path.Combine(Application.persistentDataPath, "Data", $"Slot_{slot}").Replace("\\", "/");
            if (!Directory.Exists(folderPath))
            {
                Debug.LogWarning($"Slot folder does not exist: {folderPath}");
                return;
            }

            var files = Directory.GetFiles(folderPath);
            foreach (var file in files)
            {
                try
                {
                    File.Delete(file);
                }
                catch (Exception e)
                {
                    Debug.LogError($"Failed to delete file {file}: {e.Message}");
                }
            }
        }

        public void Destroy()
        {
        }
    }
}