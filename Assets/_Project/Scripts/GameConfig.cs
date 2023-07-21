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
    }

    public enum TileState
    {
        START,
        START_TO_FLOOR,
        FLOOR,
        MOVE_TO_SLOT,
        SLOT
    }
    
    public enum GameState{
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