using System;
using System.Collections.Generic;
using UnityEngine;

namespace Match_3
{
    public class JsonBoardGame : BoardGame
    {
        public void Initialized(TileJsonData tileJsonData, Tile tile, SlotTransform slotTransformPrefab)
        {
            
            List<Transform> _listFloor = new List<Transform>();

            for (int i = 0; i < tileJsonData.Floor.Count; i++)
            {
                GameObject floor = new GameObject($"Floor {tileJsonData.Floor[i].Index}");
                floor.transform.SetParent(transform);    
                floor.transform.position = i % 2 == 0 ? new Vector3(0.5f, 0.5f, 0) : new Vector3(1f, 1f, 0);
                _listFloor.Add(floor.transform);
            }
            
            if (listItemData == null) listItemData = new List<ItemData>();
            listItemData.Clear();

            SlotTransform _slotTransform = Instantiate(slotTransformPrefab, transform);

            foreach (var item in DeserializeSOList(tileJsonData.ListItemData))
            {
                listItemData.Add(item);
            }
            
            InitDirections(_listFloor, listItemData, _slotTransform.SlotItemParent);
            ReadJsonData(tileJsonData, tile);
        }
        
        private List<ItemData> DeserializeSOList (string json_string)
        {
            string[] stringSeparators = new string[] { "}," };
            List<ItemData> result = new List<ItemData>();
            string[] splitted = json_string.Split(stringSeparators, StringSplitOptions.None);
            for(int i =0; i < splitted.Length-1; i++)
            {
                string SO_string = splitted[i] + "}";
                ItemData itemBasic = (ItemData)ScriptableObject.CreateInstance(typeof(ItemData));
                JsonUtility.FromJsonOverwrite(SO_string, itemBasic);
                result.Add(itemBasic);
            }
            return result;
        }

        protected override void ReadJsonData(TileJsonData tileJsonData, Tile tile)
        {
            ListTileData.Clear();
            
            for (int i = 0; i < tileJsonData.Floor.Count; i++)
            {
                for (int j = 0; j < tileJsonData.Floor[i].Items.Count; j++)
                {
                    Vector2Int posTile = new Vector2Int(tileJsonData.Floor[i].Items[j].X, tileJsonData.Floor[i].Items[j].Y);
                    //Debug.Log($"[Pos] : " + posTile);
                    TileData tileData = new TileData(i,  tileJsonData.Floor[i].Index, posTile);
                    ListTileData.Add(tileData);
                }
            }

            if (listItemData.Count < 3)
            {
                Debug.LogError("Coundn't load list item data less than 3");
                return;
            }
            base.ReadJsonData(tileJsonData, tile);
        }
    }
}
