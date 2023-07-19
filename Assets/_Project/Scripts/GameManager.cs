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
            if (PlayerPrefs.HasKey("~~level~~"))
            {
                _currentLevel = PlayerPrefs.GetInt("~~level~~");
            }

            _levelObject = Instantiate(Resources.Load<BoardGame>("Levels/Level" + _currentLevel));
            if (_levelObject != null)
            {
                Debug.Log(_levelObject);
                UIManager.Current.SetLevelText(_currentLevel);
            }

            if (_levelObject == null)
            {
                _currentLevel = 0;
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
    }
}