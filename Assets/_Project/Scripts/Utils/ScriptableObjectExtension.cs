using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match_3
{
    public static class ScriptableObjectExtension
    {
        public static string Serialize<T>(this List<T> list)
        {
            string result = "";
            foreach (T item in list)
            {
                result += JsonUtility.ToJson(item) + ", ";
            }
            
            return result;
        }
    }
}
