using UnityEngine;

namespace Match_3
{
    [CreateAssetMenu(menuName = "TileType", fileName = "ScriptableObjects/ItemData", order = 1)]
    public class ItemData : ScriptableObject
    {
       public TileType tileType;
    }
}