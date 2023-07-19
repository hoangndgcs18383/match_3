using System;

namespace Match_3
{
    [Serializable]
    public class GridPosition
    {
        public int x;
        public int y;

        public GridPosition(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
    }
}