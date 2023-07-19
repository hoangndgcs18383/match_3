#if UNITY_EDITOR

using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using Sirenix.OdinInspector.Editor;
using UnityEngine;

namespace Match_3
{
    public enum GridLayerEnum
    {
        FLOOR_1,
        FLOOR_2,
        FLOOR_3,
        FLOOR_4,
        FLOOR_5,
    }

    [Serializable]
    [CreateAssetMenu(menuName = "Game/GridLayer", fileName = "GridLayer")]
    public class GridLayer : SerializedScriptableObject
    {
        [OnValueChanged("OnGridLayerChanged")] public GridLayerEnum gridLayerEnum;
        public int maxGridWidth = 10;
        public int maxGridHeight = 10;

        public int layerIndex;

        [OnCollectionChanged("OnGridChanged")] [TableMatrix(SquareCells = true)]
        public Texture2D[,] gridPositionsTexture;

        private Dictionary<GridLayerEnum, Texture2D[,]> _gridLayerTexture =
            new Dictionary<GridLayerEnum, Texture2D[,]>();

        [OnInspectorInit]
        public void CreateData()
        {
            gridPositionsTexture ??= new Texture2D[maxGridWidth, maxGridHeight];
        }

        public void OnGridLayerChanged()
        {
            if ((int)gridLayerEnum % 2 != 0)
            {
            }
            else
            {
            }

            if (_gridLayerTexture.ContainsKey(gridLayerEnum))
            {
                gridPositionsTexture = _gridLayerTexture[gridLayerEnum];
            }
            else
            {
                gridPositionsTexture = new Texture2D[maxGridWidth, maxGridHeight];
            }
        }

        public void OnGridChanged(CollectionChangeInfo info)
        {
            if (_gridLayerTexture.ContainsKey(gridLayerEnum))
                _gridLayerTexture[gridLayerEnum] = gridPositionsTexture;
            else
                _gridLayerTexture.Add(gridLayerEnum, gridPositionsTexture);
        }
    }
}

#endif