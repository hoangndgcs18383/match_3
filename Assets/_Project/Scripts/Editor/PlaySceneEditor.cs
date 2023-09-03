#if UNITY_EDITOR
using System.Collections;
using Proyecto26;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zeff.Core.EndPoint;

namespace Match_3
{
    public class PlaySceneEditor : Editor
    {
        public static void Play()
        {
            EditorApplication.isPlaying = true;

            SceneManager.LoadScene("Loading");
        }

        [MenuItem("Design/GetGoogleSheet")]
        public static void GetGoogleSheet()
        {
            RestClient.Get(EndPointConstants.QUEST_URL).Then(response => { Debug.Log(response.Text); });
            
            
        }
    }
}
#endif