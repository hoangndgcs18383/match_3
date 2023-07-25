#if UNITY_EDITOR

using Sirenix.OdinInspector;
using UnityEngine;

namespace Match_3
{
    [CreateAssetMenu(menuName = "GameLevelConfig", fileName = "ScriptableObjects/GameLevelConfig")]
    public class GameLevelConfig : SerializedScriptableObject
    {
        [BoxGroup("Level Config")]
        [Title("Level Config")] [SerializeField]
        [Range(0, 1000)]
        private int startLevel;

        [SerializeField] private Grid[] levelPrefab;

        [SerializeField] private BoardGame[] levelPrefabs;

        [Button(ButtonSizes.Medium)]
        public void ValidateLevels()
        {
            foreach (var levelPrefab in levelPrefabs)
            {
                if (levelPrefab.CheckValid()) continue;
            }
        }

        [BoxGroup("Level Config")]
        [Button(ButtonSizes.Medium)]
        [InfoBox("Load level at start")]
        public void LoadLevel()
        {
            /*if (levelPrefabs.Length <= startLevel)
            {
                Debug.LogError("Level not found");
                return;
            }*/

            GameManager.Current.SetLevel(startLevel);
        }
    }
}
#endif