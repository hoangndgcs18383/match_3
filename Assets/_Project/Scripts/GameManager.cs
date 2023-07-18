using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

namespace Match_3
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Current;

        [Title("Config")] public TileSlot itemTileSlotPrefab;
        public Transform slotParentTranform;
        public int maxSlot = 7;
        public int matchTile = 3;

        public Sprite[] spriteTiles;

        public List<TileSlot> ListSlots { get; } = new List<TileSlot>();
        public List<TileDirection> ListDirections { get; } = new List<TileDirection>();

        private void Awake()
        {
            Current = this;
        }

        public void AddTileToSlot(Tile tile)
        {
            if (ListSlots.Count < maxSlot)
            {
                int indexAdd = FindIndexToAdd(tile);
                TileSlot tileSlot = Instantiate(itemTileSlotPrefab, slotParentTranform);
                tileSlot.transform.localPosition = new Vector3(-3 * 1.2f + (indexAdd * 100) * 1.2f, 0f, 0f);
                tile.transform.SetParent(tileSlot.transform);

                tileSlot.SetTile(tile);
                tile.TweenPlayMoveToSlot(indexAdd);
                ListSlots.Insert(indexAdd, tileSlot);
                
                //Reset new pos
                
            }
        }
        
        public void OnCompleteMoveToSlot(Tile tile)
        {
            //check rules > max slot => lose

            List<TileSlot> currentSlots = FindMatchThreeOfSlots(tile, out bool isMatch);

            if (isMatch)
            {
                foreach (var slot in currentSlots)
                {
                    ListSlots.Remove(slot);
                    slot.gameObject.SetActive(false);
                }
                //Check win
            }
        }

        private int FindIndexToAdd(Tile tile)
        {
            int index = ListSlots.Count;
            for (int i = ListSlots.Count - 1; i >= 0; i--)
            {
                if (ListSlots[i].Tile.data.ItemData.tileType == tile.data.ItemData.tileType)
                {
                    return i + 1;
                }
            }

            return index;
        }

        private List<TileSlot> FindMatchThreeOfSlots(Tile tile, out bool isMatch)
        {
            List<TileSlot> listMatch = new List<TileSlot>();

            for (int i = 0; i < ListSlots.Count; i++)
            {
                if(ListSlots[i].Tile.data.ItemData.tileType == tile.data.ItemData.tileType)
                    listMatch.Add(ListSlots[i]);

                if (listMatch.Count == matchTile)
                {
                    isMatch = true;
                    return listMatch;
                }
            }

            isMatch = false;
            return listMatch;
        }
        
    }
}