using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace Match_3
{
    public class BoardGame : MonoBehaviour
    {
        
        [InfoBox("List tilemap must be same with list floor transform")]
        public List<Tilemap> listTileMap;

        [InfoBox("List item data must be more than 3")]
        public List<ItemData> listItemData;

        [InfoBox("List floor transform must be same with list tilemap")]
        public List<Transform> listFloorTransform;

        public GameObject gridObject;
        
        public Tile tilePrefab;
        public Transform slotTransform;

        private List<TileData> _listTileData = new List<TileData>();

        public void Initialized()
        {
            //slotTransform.transform.l
        }

        private void Start()
        {
            InitDirections();
            ReadDataMap();
        }

        private void InitDirections()
        {
            List<TileDirection> directionCached = new List<TileDirection>();
            //shuffle list
            directionCached.Add(TileDirection.TOP);
            directionCached.Add(TileDirection.BOTTOM);
            directionCached.Add(TileDirection.RIGHT);
            directionCached.Add(TileDirection.LEFT);

            List<TileDirection> tileDirections = new List<TileDirection>();
            for (int i = 0; i < directionCached.Count; i++)
            {
                TileDirection direction = directionCached[Random.Range(0, directionCached.Count)];
                tileDirections.Add(direction);
                directionCached.Remove(direction);
            }

            for (int i = 0; i < tileDirections.Count; i++)
            {
                GameManager.Current.ListDirections.Add(tileDirections[i % 4]);
            }
        }

        private void ReadDataMap()
        {
            // Read data from json file
            _listTileData.Clear();

            int indexOnMap = 0;
            for (int i = 0; i < listTileMap.Count; i++) // Loop qua toan bo tile map
            {
                for (int y = listTileMap[i].cellBounds.yMin; y < listTileMap[i].cellBounds.yMax; y++) // loop qua toan bo y
                {
                    for (int x = listTileMap[i].cellBounds.xMin; x < listTileMap[i].cellBounds.yMax; x++) // loop qua toan bo x
                    {
                        Debug.Log("i: " + i + " y: " + y + " x: " + x);

                        if (listTileMap[i].HasTile(new Vector3Int(x, y, 0)))
                        {
                            // Neu co tile thi add vao list
                            indexOnMap++;
                            TileData tileData = new TileData(i, indexOnMap, new Vector2Int(x, y));
                            _listTileData.Add(tileData);
                        }
                    }
                }
            }

            gridObject.gameObject.SetActive(false);
            GenerateItem();
        }


        private void GenerateItem()
        {
            // Generate item
            Debug.Log($"GenerateItem {_listTileData.Count}");

            List<ItemData> listItemGenerate = new List<ItemData>();

            if (_listTileData.Count % 3 != 0)
            {
                Debug.LogError("listTileData must be more than 3"); // rule more than 3
                return;
            }

            List<ItemData> listShuffleItem = new List<ItemData>();
            List<ItemData> listItemCached = new List<ItemData>(listItemData);

            for (int i = 0; i < listItemData.Count; i++)
            {
                ItemData itemData = listItemCached[Random.Range(0, listItemCached.Count)];
                listShuffleItem.Add(itemData);
                listItemCached.Remove(itemData);
            }

            for (int i = 0; i < _listTileData.Count / 3; i++)
            {
                // Get 3 phan tu [0 / 3] [3 / 6] [6 / 9] 
                ItemData itemData = listShuffleItem[i % listShuffleItem.Count];

                //Add 3 phan tu
                listItemGenerate.Add(itemData);
                listItemGenerate.Add(itemData);
                listItemGenerate.Add(itemData);
            }

            for (int i = 0; i < _listTileData.Count; i++)
            {
                //tip tuc shuffle
                ItemData itemData = listItemGenerate[Random.Range(0, listItemGenerate.Count)];
                _listTileData[i].ItemData = itemData;
                listItemGenerate.Remove(itemData);
            }

            GenerateFloorItem();
        }

        private void GenerateFloorItem()
        {
            Debug.Log($"GenerateFloorItem {_listTileData.Count}");

            for (int i = 0; i < _listTileData.Count; i++)
            {
                Tile itemTile = Instantiate(tilePrefab, listFloorTransform[_listTileData[i].FloorIndex]);
                itemTile.Initialized(_listTileData[i]);
                itemTile.name = "" + i;
            }
        }
    }
}