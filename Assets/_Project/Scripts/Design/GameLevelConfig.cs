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
        [Range(1, 1000)]
        private int startLevel;
        
        [BoxGroup("Level Config")]
        [Button(ButtonSizes.Medium)]
        [InfoBox("Load level at start")]
        public void LoadLevel()
        {
            GameManager.Current.SetLevel(startLevel);
        }
    }
}
#endif