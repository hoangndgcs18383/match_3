#if UNITY_EDITOR

using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEngine;

namespace Match_3
{
    [CreateAssetMenu(menuName = "GameLevelConfig", fileName = "ScriptableObjects/GameLevelConfig")]
    public class GameLevelConfig : SerializedScriptableObject
    {
        private List<BoardGame> listBoardGame = new List<BoardGame>();
        
        [Button]
        [GUIColor(0.5f, 0.5f, 1f)]
        private void ExportLevelsToJson()
        {
            listBoardGame.Clear();

            foreach (var boardGame in Resources.LoadAll<BoardGame>("Levels"))
            {
                if(!boardGame.name.Contains("Level")) continue;
                if (boardGame != null)
                {
                    boardGame.WriteToJson();
                }
            }
            
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
    }
}
#endif