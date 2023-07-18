using UnityEngine;

namespace Match_3
{
    public enum TileDirection
    {
        TOP,
        RIGHT,
        BOTTOM,
        LEFT
    }

    public enum TileType
    {
        TILE_1,
        TILE_2,
        TILE_3,
    }

    public enum TileState
    {
        START,
        START_TO_FLOOR,
        FLOOR,
        MOVE_TO_SLOT,
        SLOT
    }

    public class GameConfig : MonoBehaviour
    {
    }
}