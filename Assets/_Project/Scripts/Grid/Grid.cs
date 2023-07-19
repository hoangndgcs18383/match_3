#if UNITY_EDITOR
using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match_3
{
    [Serializable]
    [CreateAssetMenu(menuName = "Game/Grid", fileName = "Grid")]
    public class Grid : SerializedScriptableObject
    {
        public GridLayer[] gridLayer;
    }
}
#endif
