using UnityEngine;

namespace Zeff.Extensions
{
    public static class MathExtenstion
    {
        public static Vector3 TileToLeft(float tileSize, Vector2Int posTile, int floorIndex)
        {
            return new Vector3(tileSize * posTile.x - 12f, tileSize * posTile.y, floorIndex);
        }

        public static Vector3 TileToRight(float tileSize, Vector2Int posTile, int floorIndex)
        {
            return new Vector3(tileSize * posTile.x + 12f, tileSize * posTile.y, floorIndex);
        }

        public static Vector3 TileToTop(float tileSize, Vector2Int posTile, int floorIndex)
        {
            return new Vector3(tileSize * posTile.x, tileSize * posTile.y - 20f, floorIndex);
        }

        public static Vector3 TileToBottom(float tileSize, Vector2Int posTile, int floorIndex)
        {
            return new Vector3(tileSize * posTile.x, tileSize * posTile.y + 20f, floorIndex);
        }
    }
}