using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Tilemaps;
using Random = UnityEngine.Random;

namespace Match_3
{
    public class BoardGame : MonoBehaviour
    {
        [InfoBox("List tilemap must be same with list floor transform")]
        public List<Tilemap> listTileMap;

        [TabGroup("Design Items")] [InfoBox("List item data must be more than 3")]
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
            GameManager.Current.ListDirections.Clear();
            List<TileDirection> directionCached = new List<TileDirection>
            {
                //shuffle list
                TileDirection.TOP,
                TileDirection.BOTTOM,
                TileDirection.RIGHT,
                TileDirection.LEFT
            };

            List<TileDirection> tileDirections = new List<TileDirection>();
            for (int i = 0; i < 4; i++)
            {
                TileDirection direction = directionCached[Random.Range(0, directionCached.Count)];
                tileDirections.Add(direction);
                directionCached.Remove(direction);
            }

            for (int i = 0; i < listFloorTransform.Count; i++)
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
                for (int y = listTileMap[i].cellBounds.yMin;
                     y < listTileMap[i].cellBounds.yMax;
                     y++) // loop qua toan bo y
                {
                    for (int x = listTileMap[i].cellBounds.xMin;
                         x < listTileMap[i].cellBounds.yMax;
                         x++) // loop qua toan bo x
                    {
                        //Debug.Log("i: " + i + " y: " + y + " x: " + x);

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
            //Debug.Log($"GenerateItem {_listTileData.Count}");

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
                itemTile.name = $"{_listTileData[i].FloorIndex} floor | {_listTileData[i].IndexOnMap} index _ " + i;
            }

            GameManager.Current.GameState = GameState.PLAYING;
        }

        public void OnMoveComplete()
        {
            StartCoroutine(IECheckWin());
        }


        private IEnumerator IECheckWin()
        {
            if (GameManager.Current.GameState == GameState.WIN)
                yield break;
            if (CheckWin())
            {
                GameManager.Current.GameState = GameState.WIN;

                yield return new WaitForSeconds(0.5f);

                UIManager.Current.ShowPopup("You Win", "Next Level", () => { GameManager.Current.LoadNextLevel(); },
                    () => { GameManager.Current.RestartLevel(); });
            }
        }

        public bool CheckWin()
        {
            for (int i = 0; i < listFloorTransform.Count; i++)
            {
                if (listFloorTransform[i].transform.childCount > 0)
                {
                    return false;
                }
            }

            return true;
        }

        #region Shuffle

        private readonly List<Tile> _listTileShuffle = new List<Tile>();

        public void Shuffle()
        {
            _listTileShuffle.Clear();

            for (int i = 0; i < listFloorTransform.Count; i++)
            {
                foreach (Transform child in listFloorTransform[i])
                {
                    Tile tile = child.GetComponent<Tile>();
                    if (tile != null)
                    {
                        _listTileShuffle.Add(tile);
                    }
                }
            }

            List<Tile> listCached = new List<Tile>(_listTileShuffle);
            List<ItemData> listShuffle = new List<ItemData>();

            for (int i = 0; i < _listTileShuffle.Count; i++)
            {
                int index = Random.Range(0, listCached.Count);
                listShuffle.Add(listCached[index].data.ItemData);
                listCached.RemoveAt(index);
            }

            for (int i = 0; i < _listTileShuffle.Count; i++)
            {
                _listTileShuffle[i].SetShuffle(listShuffle[i]);
            }
        }

        #endregion

#if UNITY_EDITOR

        [Title("Design Map")] [PropertySpace(50)] [ReadOnly] [SerializeField]
        private int floor1;

        [ReadOnly] [SerializeField] private int floor2;
        [ReadOnly] [SerializeField] private int floor3;
        [ReadOnly] [SerializeField] private int floor4;
        [ReadOnly] [SerializeField] private int floor5;

        [OnInspectorInit]
        public void ShowInfoTileMap()
        {
            for (int i = 0; i < listTileMap.Count; i++)
            {
                for (int y = listTileMap[i].cellBounds.yMin;
                     y < listTileMap[i].cellBounds.yMax;
                     y++)
                {
                    for (int x = listTileMap[i].cellBounds.xMin;
                         x < listTileMap[i].cellBounds.yMax;
                         x++)
                    {
                        if (listTileMap[i].HasTile(new Vector3Int(x, y, 0)))
                        {
                            switch (i)
                            {
                                case 0:
                                    floor1++;
                                    break;
                                case 1:
                                    floor2++;
                                    break;
                                case 2:
                                    floor3++;
                                    break;
                                case 3:
                                    floor4++;
                                    break;
                                case 4:
                                    floor5++;
                                    break;
                            }
                        }
                    }
                }
            }
        }

        [PropertySpace(10)]
        [TabGroup("Design Map", true, 1)]
        [GUIColor(1, 1, 1, 0.75f)]
        [Button(ButtonSizes.Large)]
        public void OnScreenCheckValid()
        {
            if (CheckValid())
            {
                Debug.Log($"Valid at {gameObject.name}");
            }
            else
            {
                Debug.LogError($"Invalid at {gameObject.name}");
            }
        }

        [PropertySpace(10)]
        [TabGroup("Design Map", true, 1)]
        [GUIColor(1, 1, 0.75f, 0.75f)]
        [Button(ButtonSizes.Large)]
        public void OnScreenClearMap()
        {
            ClearTileMap();
        }

        public bool CheckValid()
        {
            List<TileData> listTileMapCached = new List<TileData>();

            int indexOnMap = 0;
            for (int i = 0; i < listTileMap.Count; i++) // Loop qua toan bo tile map
            {
                for (int y = listTileMap[i].cellBounds.yMin;
                     y < listTileMap[i].cellBounds.yMax;
                     y++) // loop qua toan bo y
                {
                    for (int x = listTileMap[i].cellBounds.xMin;
                         x < listTileMap[i].cellBounds.yMax;
                         x++) // loop qua toan bo x
                    {
                        //Debug.Log("i: " + i + " y: " + y + " x: " + x);

                        if (listTileMap[i].HasTile(new Vector3Int(x, y, 0)))
                        {
                            // Neu co tile thi add vao list
                            indexOnMap++;
                            TileData tileData = new TileData(i, indexOnMap, new Vector2Int(x, y));
                            listTileMapCached.Add(tileData);
                        }
                    }
                }
            }

            if (listTileMapCached.Count % 3 != 0)
            {
                Debug.LogWarning($"Invalid: Tile count is not divisible by 3, đang dư {listTileMapCached.Count % 3}");
                return false;
            }

            return true;
        }

        public void ClearTileMap()
        {
            for (int i = 0; i < listTileMap.Count; i++)
            {
                listTileMap[i].ClearAllTiles();
            }
        }
#endif
    }
}