using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;

namespace Match_3
{
    public class GameManager : MonoBehaviour
    {
        public static GameManager Current;

        [Title("Config")] public TileSlot itemTileSlotPrefab;

        //public Transform slotParentTranform;
        public int maxSlot = 7;
        public int matchTile = 3;

        public SpriteAtlas spriteTiles;

        private int _currentLevel = 1;
        private BoardGame _levelObject;

        public List<TileSlot> ListSlots { get; } = new List<TileSlot>();
        public List<TileDirection> ListDirections { get; set; } = new List<TileDirection>();
        public GameState GameState { get; set; }

        private void Awake()
        {
            Current = this;
        }

        private void Start()
        {
            SetTargetFPS();
            LoadLevel();
            UpdateCoinView();
        }

        private void SetTargetFPS()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
        
        #region Stuffs
        
        private void UpdateCoinView()
        {
            UIManager.Current.SetCoinText(GetCoin());
        }

        public void AddCoin(int coin)
        {
            PlayerPrefs.SetInt(GameConfig.COIN, GetCoin() + coin);
            PlayerPrefs.Save();
            UpdateCoinView();
        }

        public int GetCoin()
        {
            return PlayerPrefs.GetInt(GameConfig.COIN);
        }

        #endregion

        #region Board

        private void LoadLevel()
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
                _levelObject.Initialized();
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
                tileSlot.transform.localPosition = GameConfig.GetMoveTile(indexAdd);
                tile.transform.SetParent(tileSlot.transform);

                tileSlot.SetTile(tile);
                tile.TweenPlayMoveToSlot(indexAdd);
                ListSlots.Insert(indexAdd, tileSlot);

                Timing.RunCoroutine(IEResetPosition(0.1f));
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
                    slot.SetMatchCallback();
                }

                //Check win
                Timing.RunCoroutine(IEResetPosition(0.3f));
                _levelObject.OnMoveComplete();
            }
            else
            {
                //Check lose
                _levelObject.OnMoveComplete(ListSlots, RestartLevel, RestartLevel);
            }
        }

        private IEnumerator<float> IEResetPosition(float time)
        {
            yield return Timing.WaitForSeconds(time);
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

        #endregion

        #region Gameplay

        #region Shuffle

        public void OnButtonShuffle()
        {
            if (GameState == GameState.PLAYING)
            {
                _levelObject.Shuffle();
            }
        }

        #endregion

        #region Suggest

        public void OnButtonSuggest()
        {
            if (GameState == GameState.PLAYING)
            {
                _levelObject.Suggest();
            }
        }

        #endregion

        #endregion

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
            GC.Collect();
            GC.WaitForPendingFinalizers();
            Resources.UnloadUnusedAssets();
        }

        private void OnDestroy()
        {
            Timing.KillCoroutines();
            DOTween.KillAll();
        }

        #endregion
    }
}