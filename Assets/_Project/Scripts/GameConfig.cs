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
        TILE_1 = 1,
        TILE_2 = 2,
        TILE_3 = 3,
        TILE_4 = 4,
        TILE_5 = 5,
        TILE_6 = 6,
        TILE_7 = 7,
        TILE_8 = 8,
        TILE_9 = 9,
        TILE_10 = 10,
        TILE_11 = 11,
        TILE_12 = 12,
        TILE_13 = 13,
        TILE_14 = 14,
        TILE_15 = 15,
        TILE_16 = 16,
        TILE_17 = 17,
        TILE_18 = 18,
        TILE_19 = 19,
        TILE_20 = 20,
    }

    public enum TileState
    {
        START,
        START_TO_FLOOR,
        FLOOR,
        MOVE_TO_SLOT,
        SLOT
    }

    public enum GameState
    {
        START,
        PLAYING,
        END,
        WIN,
        LOSE,
        PAUSE,
    }

    public class GameConfig : MonoBehaviour
    {
    }
}