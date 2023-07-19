#if UNITY_EDITOR

using Sirenix.OdinInspector;
using UnityEngine;

namespace Match_3
{
    [CreateAssetMenu(menuName = "GameLevelConfig", fileName = "ScriptableObjects/GameLevelConfig")]
    public class GameLevelConfig : SerializedScriptableObject
    {
        [SerializeField] private Grid[] levelPrefab;

        [SerializeField] private BoardGame[] levelPrefabs;

        [Button(ButtonSizes.Medium)]
        public void ValidateLevels()
        {
            foreach (var levelPrefab in levelPrefabs)
            {
                if (levelPrefab.CheckValid()) continue;

                Debug.LogError($"Level {levelPrefab.name} is not valid");
            }
        }
    }
}
#endif
