using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Match_3
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Current;

        [Title("Config")] public TileSlot itemTileSlotPrefab;

        //public Transform slotParentTranform;
        public int maxSlot = 7;
        public int matchTile = 3;

        public Sprite[] spriteTiles;

        private int _currentLevel = 1;
        private BoardGame _levelObject;

        public List<TileSlot> ListSlots { get; } = new List<TileSlot>();
        public List<TileDirection> ListDirections { get; set; } = new List<TileDirection>();

        public List<Transform> ListFloorTransform { get; } = new List<Transform>();

        public GameState GameState { get; set; }

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            LoadLevel();
        }

        public void LoadLevel()
        {
            GameState = GameState.START;

            if (PlayerPrefs.HasKey("~~level~~"))
            {
                _currentLevel = PlayerPrefs.GetInt("~~level~~");
            }

            BoardGame boardGame = Resources.Load<BoardGame>("Levels/Level" + _currentLevel);

            if (boardGame != null)
            {
                _levelObject = Instantiate(boardGame);
                UIManager.Current.SetLevelText(_currentLevel);
            }
            else
            {
                _currentLevel--;
                _levelObject = Instantiate(Resources.Load<BoardGame>("Levels/Level" + _currentLevel));
            }
        }

        public void AddTileToSlot(Tile tile)
        {
            if (ListSlots.Count < maxSlot)
            {
                int indexAdd = FindIndexToAdd(tile);
                TileSlot tileSlot = Instantiate(itemTileSlotPrefab, _levelObject.slotTransform);
                tileSlot.transform.localPosition = new Vector3(-3 * 1.2f + indexAdd * 1.2f, 0f, 0f);
                tile.transform.SetParent(tileSlot.transform);

                tileSlot.SetTile(tile);
                tile.TweenPlayMoveToSlot(indexAdd);
                ListSlots.Insert(indexAdd, tileSlot);

                StartCoroutine(IEResetPosition(0.1f));
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
                StartCoroutine(IEResetPosition(0.3f));
                _levelObject.OnMoveComplete();
            }
            else
            {
                //Check lose
                StartCoroutine(IECheckLose());
            }
        }

        private IEnumerator IECheckLose()
        {
            yield return new WaitForSeconds(0.3f);
            if (CheckLose())
            {
                UIManager.Current.ShowPopup("You Lose", "Try again",
                    RestartLevel, Application.Quit);
            }
        }

        private bool CheckLose()
        {
            if (ListSlots.Count < maxSlot)
                return false;

            for (int i = 0; i < ListSlots.Count; i++)
            {
                int count = CountItemHasData(ListSlots[i].Tile.data.ItemData);
                if (count == 3)
                    return false;
            }

            return true;
        }

        private int CountItemHasData(ItemData data)
        {
            int count = 0;
            for (int i = 0; i < ListSlots.Count; i++)
            {
                if (ListSlots[i].Tile.data.ItemData.tileType == data.tileType)
                    count++;
            }

            return count;
        }

        private IEnumerator IEResetPosition(float time)
        {
            yield return new WaitForSeconds(time);
            for (int i = 0; i < ListSlots.Count; i++)
            {
                ListSlots[i].ResetPosSlot(i);
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
                if (ListSlots[i].Tile.data.ItemData.tileType == tile.data.ItemData.tileType)
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

        #region Level

        public void SetLevel(int level)
        {
            _currentLevel = level;
            SaveLevel();
        }

        private void SaveLevel()
        {
            PlayerPrefs.SetInt("~~level~~", _currentLevel);
            PlayerPrefs.Save();
        }

        public void LoadNextLevel()
        {
            _currentLevel++;
            SaveLevel();
            SceneManager.LoadScene("Main");
            GCCollectAndClear();
        }


        public void RestartLevel()
        {
            SceneManager.LoadScene("Main");
            GCCollectAndClear();
        }

        private void GCCollectAndClear()
        {
            System.GC.Collect();
            System.GC.WaitForPendingFinalizers();
            Resources.UnloadUnusedAssets();
        }

        #endregion
    }
}