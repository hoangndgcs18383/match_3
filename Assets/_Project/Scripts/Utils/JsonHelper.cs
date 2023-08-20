using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Networking;

namespace Match_3
{
    public static class JsonHelper
    {
        public static string TryToReadJson(string path)
        {
            string dataPath = Path.Combine(Application.dataPath, path);
            
            try
            {
                return File.ReadAllText(dataPath);
            }
            catch (Exception e)
            {
                TextAsset jsonAsset = Resources.Load<TextAsset>(dataPath);
                if (jsonAsset != null)
                {
                    Debug.LogError("Failed to load JSON file: " + e.Message);
                    return jsonAsset.text;
                }

                return "";
            }
        }
        
        public static string TryToLoadJsonAsync(string path)
        {
            string dataPath = Path.Combine(Application.dataPath, path);
            
            try
            {
                return File.ReadAllText(dataPath);
            }
            catch (Exception e)
            {
                TextAsset jsonAsset = Resources.Load<TextAsset>(dataPath);
                if (jsonAsset != null)
                {
                    return jsonAsset.text;
                }

                return "";
            }
            
        }

        private static IEnumerator IELoadJson(string path)
        {
            string filePath = Path.Combine(Application.streamingAssetsPath, path);

            UnityWebRequest www = UnityWebRequest.Get(filePath);
            yield return www.SendWebRequest();

            if (www.result == UnityWebRequest.Result.Success)
            {
                string jsonData = www.downloadHandler.text;
                
            }
            else
            {
                Debug.LogError("Failed to load JSON file: " + www.error);
            }
        } 
    }
}