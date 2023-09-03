using System;
using System.IO;
using UnityEngine;
using Zeff.Core.Parser;

namespace Zeff.Core.SaveGame
{
    public enum DataPath
    {
        Default,
        PersistentDataPath,
        StreamingAssetsPath,
        TemporaryCachePath
    }

    [Serializable]
    public class ZSaveGame
    {
        private const string SAVE_FILE_NAME = "save.json";

        public ZSaveGame()
        {
        }

        public static void CreateDefault<T>(T defaultData, string key, DataPath dataPath = DataPath.Default)
            where T : class
        {
            switch (dataPath)
            {
                case DataPath.Default:
                    SaveLocal(defaultData, key);
                    break;
                case DataPath.PersistentDataPath:
                    SaveLocal(defaultData, key, DataPath.PersistentDataPath);
                    break;
                case DataPath.StreamingAssetsPath:
                    SaveLocal(defaultData, key, DataPath.StreamingAssetsPath);
                    break;
                case DataPath.TemporaryCachePath:
                    SaveLocal(defaultData, key, DataPath.TemporaryCachePath);
                    break;
            }

            PlayerPrefs.Save();
        }

        public static void SaveLocal<T>(T data, string key, DataPath dataPath) where T : class
        {
            switch (dataPath)
            {
                case DataPath.Default:
                    SaveLocal(data, key);
                    break;
                case DataPath.PersistentDataPath:
                    SaveLocalPersistentDataPath(data, key);
                    break;
                case DataPath.StreamingAssetsPath:
                    SaveStreamingAssets(data, key);
                    break;
                case DataPath.TemporaryCachePath:
                    SaveTemporaryCache(data, key);
                    break;
            }
        }

        private static void SaveTemporaryCache<T>(T data, string key) where T : class
        {
            string filePath = Path.Combine(Application.temporaryCachePath, SAVE_FILE_NAME);

            try
            {
                File.WriteAllText(filePath, data.ToJson());
            }
            catch (Exception e)
            {
                try
                {
                    PlayerPrefs.SetString(key, data.ToJson());
                    PlayerPrefs.Save();
                }
                catch (Exception exception)
                {
                    Debug.LogError("[ZSaveGame] SaveLocal: " + exception);
                }
            }
        }

        private static void SaveStreamingAssets<T>(T data, string key) where T : class
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, SAVE_FILE_NAME);

            try
            {
                File.WriteAllText(filePath, data.ToJson());
            }
            catch (Exception e)
            {
                try
                {
                    PlayerPrefs.SetString(key, data.ToJson());
                    PlayerPrefs.Save();
                }
                catch (Exception exception)
                {
                    Debug.LogError("[ZSaveGame] SaveLocal: " + exception);
                }
            }
        }

        public static void SaveLocalPersistentDataPath<T>(T data, string key) where T : class
        {
            string filePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);

            try
            {
                File.WriteAllText(filePath, data.ToJson());
            }
            catch (Exception e)
            {
                try
                {
                    PlayerPrefs.SetString(key, data.ToJson());
                    PlayerPrefs.Save();
                }
                catch (Exception exception)
                {
                    Debug.LogError("[ZSaveGame] SaveLocal: " + exception);
                }
            }
        }

        public static void SaveLocal<T>(T data, string key) where T : class
        {
            string filePath = Path.Combine(Application.dataPath, SAVE_FILE_NAME);

            try
            {
                File.WriteAllText(filePath, data.ToJson());
            }
            catch (Exception e)
            {
                try
                {
                    PlayerPrefs.SetString(key, data.ToJson());
                    PlayerPrefs.Save();
                }
                catch (Exception exception)
                {
                    Debug.LogError("[ZSaveGame] SaveLocal: " + exception);
                }
            }
        }

        public static void LoadLocal<T>(string key, Action<T> callback) where T : class
        {
#if UNITY_EDITOR
            string filePath = Path.Combine(Application.dataPath, SAVE_FILE_NAME);
#else
            string filePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
#endif

            try
            {
                var json = File.ReadAllText(filePath);
                var data = json.FromJson<T>();
                callback?.Invoke(data as T);
            }
            catch (Exception e)
            {
                try
                {
                    var json = PlayerPrefs.GetString(key);
                    var data = json.FromJson<T>();
                    callback?.Invoke(data as T);
                }
                catch (Exception exception)
                {
                    Debug.LogError("[ZSaveGame] LoadLocal: " + exception);
                }
            }
        }

        public static bool HasKey(string key)
        {
            try
            {
#if UNITY_EDITOR
                string filePath = Path.Combine(Application.dataPath, SAVE_FILE_NAME);
#else
                string filePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
#endif
                return File.Exists(filePath);

            }
            catch (Exception e)
            {
                return PlayerPrefs.HasKey(key);
            }
        }

        public static void DeleteAll()
        {
            PlayerPrefs.DeleteAll();
#if UNITY_EDITOR
            string filePath = Path.Combine(Application.dataPath, SAVE_FILE_NAME);
#else
            string filePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
#endif
            File.Delete(filePath);
        }
    }
}