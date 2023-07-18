using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Match_3
{
    public class TileSlot : MonoBehaviour
    {
        private Tile _tile;
        
        public Tile Tile => _tile;

        public void SetTile(Tile tile)
        {
            _tile = tile;
        }
        
        public void MatchingCallback()
        {
            
        }
    }
}