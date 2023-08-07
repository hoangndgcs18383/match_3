using System;
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

    [Serializable]
    public class PowerUpData
    {
        public PowerUpType powerUpType;
        public int count;
    }

    public static class GameConfig
    {
        public static string COIN = "COIN";
        public static float TILE_SIZE = 1.25f;
        public static float TILE_MOVE_SIZE = 1.1f;
        public static int DEFAULT_SHUFFLE_COUNT = PlayerPrefs.GetInt("ShuffleCount", 3);
        public static int DEFAULT_SUGGESTS_COUNT = PlayerPrefs.GetInt("SuggestsCount", 3);
        public static int DEFAULT_UNDO_COUNT = PlayerPrefs.GetInt("UndoCount", 3);


        public static Vector3 GetMoveTile(int index)
        {
            return new Vector3(-3 * TILE_MOVE_SIZE + index * TILE_MOVE_SIZE, 0f, 0f);
        }
        
        public static Vector3 GetAddTile(int index)
        {
            return new Vector3(-3 * TILE_SIZE + index * TILE_SIZE, 0f, 0f);
        }

        public static int GetPowerUpCount(PowerUpType powerUpPowerUpType)
        {
            switch (powerUpPowerUpType)
            {
                case PowerUpType.Shuffle:
                    return DEFAULT_SHUFFLE_COUNT;
                case PowerUpType.Suggests:
                    return DEFAULT_SUGGESTS_COUNT;
                case PowerUpType.Undo:
                    return DEFAULT_UNDO_COUNT;
            }

            return 0;
        }

        public static void UsePowerUp(PowerUpType powerUpPowerUpType)
        {
            switch (powerUpPowerUpType)
            {
                case PowerUpType.Shuffle:
                    if (DEFAULT_SHUFFLE_COUNT > 0)
                    {
                        DEFAULT_SHUFFLE_COUNT--;
                        PlayerPrefs.SetInt("ShuffleCount", DEFAULT_SHUFFLE_COUNT);
                    }

                    break;
                case PowerUpType.Suggests:
                    if (DEFAULT_SUGGESTS_COUNT > 0)
                    {
                        DEFAULT_SUGGESTS_COUNT--;
                        PlayerPrefs.SetInt("SuggestsCount", DEFAULT_SUGGESTS_COUNT);
                    }

                    break;
                case PowerUpType.Undo:
                    if (DEFAULT_UNDO_COUNT > 0)
                    {
                        DEFAULT_UNDO_COUNT--;
                        PlayerPrefs.SetInt("UndoCount", DEFAULT_UNDO_COUNT);
                    }

                    break;
            }
        }
    }
}