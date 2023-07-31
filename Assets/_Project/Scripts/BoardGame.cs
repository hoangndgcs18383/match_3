using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MEC;
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

        #region WIN CONDITION

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

                GameManager.Current.AddCoin(10);
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

        #endregion

        #region LOSE CONDITION

        public void OnMoveComplete(List<TileSlot> tileSlots, Action onOke, Action onCancel)
        {
            Timing.RunCoroutine(IECheckLose(tileSlots, onOke, onCancel));
        }

        private IEnumerator<float> IECheckLose(List<TileSlot> tileSlots, Action onOke, Action onCancel)
        {
            yield return Timing.WaitForSeconds(0.3f);
            if (CheckLose(tileSlots))
            {
                if (GameManager.Current.GameState == GameState.LOSE)
                    yield break;
                GameManager.Current.GameState = GameState.LOSE;
                UIManager.Current.ShowPopup("You Lose", "Try again",
                    onOke, onCancel);
            }
        }

        private bool CheckLose(List<TileSlot> tileSlots)
        {
            if (tileSlots.Count < GameManager.Current.maxSlot)
                return false;

            for (int i = 0; i < tileSlots.Count; i++)
            {
                int count = CountItemHasData(tileSlots, tileSlots[i].Tile.data.ItemData);
                if (count == 3)
                    return false;
            }

            return true;
        }

        private int CountItemHasData(List<TileSlot> tileSlots, ItemData data)
        {
            int count = 0;
            for (int i = 0; i < tileSlots.Count; i++)
            {
                if (tileSlots[i].Tile.data.ItemData.tileType == data.tileType)
                    count++;
            }

            return count;
        }

        #endregion

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

        #region Suggest

        private List<Tile> _listSuggestTiles = new List<Tile>();
        private Dictionary<TileType, List<Tile>> _availableDictionary = new Dictionary<TileType, List<Tile>>();
        private Dictionary<TileType, List<Tile>> _dictAllTile = new Dictionary<TileType, List<Tile>>();

        public void Suggest()
        {
            _listSuggestTiles.Clear();

            for (int i = 0; i < listFloorTransform.Count; i++)
            {
                foreach (Transform child in listFloorTransform[i])
                {
                    Tile tile = child.GetComponent<Tile>();
                    if (tile != null)
                    {
                        _listSuggestTiles.Add(tile);
                    }
                }
            }

            Debug.Log("Suggest: " + _listSuggestTiles.Count);
            SplitListSuggestTile();
            List<Tile> listSuggest = GetListSuggest(GameManager.Current.ListSlots);
            Debug.Log("listSuggest: " + listSuggest.Count);

            if (listSuggest.Count > 0)
            {
                Timing.RunCoroutine(IESetSuggest(listSuggest));
            }
        }

        private IEnumerator<float> IESetSuggest(List<Tile> listSuggest)
        {
            yield return Timing.WaitForSeconds(0.5f);
            for (int i = 0; i < listSuggest.Count; i++)
            {
                listSuggest[i].SetSuggest();
                yield return Timing.WaitForSeconds(0.1f);
            }
        }

        private void SplitListSuggestTile()
        {
            List<Tile> listCached = new List<Tile>(_listSuggestTiles);

            _availableDictionary.Clear();
            _dictAllTile.Clear();

            for (int i = listCached.Count - 1; i >= 0; i--)
            {
                //Check available
                if (listCached[i].CanTouch())
                {
                    if (_availableDictionary.ContainsKey(listCached[i].data.ItemData.tileType))
                    {
                        _availableDictionary[listCached[i].data.ItemData.tileType].Add(listCached[i]);
                    }
                    else
                    {
                        _availableDictionary.Add(listCached[i].data.ItemData.tileType,
                            new List<Tile> { listCached[i] });
                    }
                }

                //Add all
                if (_dictAllTile.ContainsKey(listCached[i].data.ItemData.tileType))
                {
                    _dictAllTile[listCached[i].data.ItemData.tileType].Add(listCached[i]);
                }
                else
                {
                    _dictAllTile.Add(listCached[i].data.ItemData.tileType, new List<Tile> { listCached[i] });
                }
            }
        }

        public List<Tile> GetListSuggest(List<TileSlot> listSlot)
        {
            List<Tile> listSuggest = new List<Tile>();

            if (listSlot.Count > 0)
            {
                listSuggest.Clear();

                List<ItemData> listCheckData = new List<ItemData>();

                for (int i = 0; i < listSlot.Count; i++)
                {
                    ItemData itemData = listSlot[i].Tile.data.ItemData;

                    if (listCheckData.Contains(itemData))
                        break;

                    listCheckData.Add(itemData);
                    int countItemData = CountingTileHaveSlotData(listSlot, itemData);

                    if (countItemData == 1)
                    {
                        if (listSlot.Count <= 5)
                        {
                            if (_availableDictionary.ContainsKey(itemData.tileType))
                            {
                                List<Tile> listCached = _availableDictionary[itemData.tileType];
                                if (listCached.Count >= 2)
                                {
                                    //add 2 item
                                    listSuggest.Add(_availableDictionary[itemData.tileType][0]);
                                    listSuggest.Add(_availableDictionary[itemData.tileType][1]);
                                    return listSuggest;
                                }
                            }
                        }
                    }
                    else if (countItemData == 2)
                    {
                        if (listSlot.Count <= 6)
                        {
                            if (_dictAllTile.ContainsKey(itemData.tileType))
                            {
                                List<Tile> listCached = _availableDictionary[itemData.tileType];
                                if (listCached.Count >= 1)
                                {
                                    listSuggest.Add(_availableDictionary[itemData.tileType][0]);
                                    return listSuggest;
                                }
                            }
                        }
                    }
                }

                return listSuggest;
            }

            if (listSlot.Count <= 4)
            {
                foreach (var item in _availableDictionary)
                {
                    if (item.Value.Count >= 3)
                    {
                        listSuggest.Add(item.Value[0]);
                        listSuggest.Add(item.Value[1]);
                        listSuggest.Add(item.Value[2]);
                        return listSuggest;
                    }
                }
            }
            else
            {
                if (listSlot.Count >= 6)
                {
                    List<ItemData> listCheckData = new List<ItemData>();

                    for (int i = 0; i < listSlot.Count; i++)
                    {
                        ItemData itemData = listSlot[i].Tile.data.ItemData;

                        if (listCheckData.Contains(itemData))
                            break;

                        listCheckData.Add(itemData);
                        int countItemData = CountingTileHaveSlotData(listSlot, itemData);

                        if (countItemData == 2)
                        {
                            List<Tile> listCached = _dictAllTile[itemData.tileType];

                            if (listCached.Count >= 1)
                            {
                                listSuggest.Add(listCached[0]);
                                return listSuggest;
                            }
                        }
                    }

                    //if slot have 6 items and still not have suggest
                    return listSuggest;
                }
                else
                {
                    for (int i = 0; i < listSlot.Count; i++)
                    {
                        ItemData itemData = listSlot[i].Tile.data.ItemData;

                        List<Tile> listCached = _dictAllTile[itemData.tileType];
                        if (listCached.Count >= 2)
                        {
                            listSuggest.Add(listCached[^1]);
                            listSuggest.Add(listCached[^2]);
                            return listSuggest;
                        }
                    }

                    return listSuggest;
                }
            }

            return listSuggest;
        }

        private int CountingTileHaveSlotData(List<TileSlot> tileSlots, ItemData data)
        {
            int count = 0;

            for (int i = 0; i < tileSlots.Count; i++)
            {
                if (tileSlots[i].Tile.data.ItemData == data)
                {
                    count++;
                }
            }

            return count;
        }

        #endregion

#if UNITY_EDITOR

        [Title("Design Map")] [PropertySpace(50)] [ReadOnly] [SerializeField]
        private int floor1;

        [ReadOnly] [SerializeField] private int floor2;
        [ReadOnly] [SerializeField] private int floor3;
        [ReadOnly] [SerializeField] private int floor4;
        [ReadOnly] [SerializeField] private int floor5;
        [ReadOnly] [SerializeField] private int totalTile;
        [ReadOnly] [SerializeField] private bool isValid;
        [GUIColor(0,1,0)]
        [ReadOnly] [SerializeField] private int needItem;
        [GUIColor(0,1,0.5f)]
        [ReadOnly] [SerializeField] private int leftOverItem;

        [OnInspectorInit]
        public void ShowInfoTileMap()
        {
            floor1 = 0;
            floor2 = 0;
            floor3 = 0;
            floor4 = 0;
            floor5 = 0;


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

            totalTile = floor1 + floor2 + floor3 + floor4 + floor5;
            isValid = CheckValid();
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
                needItem = 3 - (listTileMapCached.Count % 3);
                leftOverItem = listTileMapCached.Count % 3;
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