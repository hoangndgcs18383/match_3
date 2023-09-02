using System;
using Newtonsoft.Json;
using UnityEngine;

namespace Zeff.Core.Parser
{
    public static class ZJsonHelper
    {
        private static JsonSerializerSettings SettingDefault(Formatting format)
        {
            return new JsonSerializerSettings
            {
                DefaultValueHandling = DefaultValueHandling.Ignore,
                NullValueHandling = NullValueHandling.Ignore,
                Formatting = format
            };
        }

        public static string ToJson(this object obj, Formatting format = Formatting.None)
        {
            try
            {
                return JsonConvert.SerializeObject(obj, SettingDefault(format));
            }
            catch (Exception e1)
            {
                try
                {
                    return JsonUtility.ToJson(obj, format == Formatting.Indented);
                }
                catch (Exception e2)
                {
                    Debug.LogError($"[parser] ToString Exception: {e2}");
                }
            }

            return default;
        }
        
        
        public static object FromJson<T>(this string json) where T : class
        {
            try
            {
                return JsonConvert.DeserializeObject(json, typeof(T), SettingDefault(Formatting.None));
            }
            catch (Exception e1)
            {
                try
                {
                    return JsonUtility.FromJson(json, typeof(T));
                }
                catch (Exception e2)
                {
                    Debug.LogError($"[parser] FromJson Exception: {e2}");
                }
            }

            return default;
        }
    }
}