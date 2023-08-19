using System;
using System.Collections.Generic;
using System.IO;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.U2D;
using Random = UnityEngine.Random;

namespace Match_3
{
    public enum LoadLevelType
    {
        ManualLoad,
        JsonLoad
    }
    
    public class GameManager : MonoBehaviour
    {
        public static GameManager Current;

        [Title("Config")] public TileSlot itemTileSlotPrefab;
         public Tile tilePrefab;
         public SlotTransform slotTransformPrefab;

        //public Transform slotParentTranform;
        public int maxSlot = 7;
        public int matchTile = 3;

        public SpriteAtlas spriteTiles;
        public LoadLevelType LoadLevelType = LoadLevelType.ManualLoad;

        private int _currentLevel = 1;
        private BoardGame _levelObject;

        public List<TileSlot> ListSlots { get; } = new List<TileSlot>();
        public List<TileDirection> ListDirections { get; set; } = new List<TileDirection>();
        public GameState GameState { get; set; }

        public Camera mainCamera; 
            
        
        private void Awake()
        {
            if (Current == null)
            {
                Current = this;
                if (transform.parent == null)
                {
                    
                    DontDestroyOnLoad(this);
                }
            }
        }

        private void Start()
        {
            mainCamera = Camera.main;
            
            SetTargetFPS();
            StartLevel();
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

        public void LoadLevel()
        {
            GameState = GameState.START;
            
            if (PlayerPrefs.HasKey(StringConstants.SAVE_LEVEL))
            {
                _currentLevel = PlayerPrefs.GetInt(StringConstants.SAVE_LEVEL);
            }

            string path = "Levels/Level";

            switch (LoadLevelType)
            {
                case LoadLevelType.ManualLoad:
                    ManualLoad(path);
                    break;
                case LoadLevelType.JsonLoad:
                    JsonLoad();
                    break;
            }
            
            UIManager.Current.SetLevelText(_currentLevel);
        }

        private void ManualLoad(string path)
        {
            BoardGame boardGame = Resources.Load<BoardGame>(path + _currentLevel);

            if (boardGame != null)
            {
                _levelObject = Instantiate(boardGame);
                _levelObject.Initialized();
            }
            else
            {
                _currentLevel = Random.Range(0, 50);
                _levelObject = Instantiate(Resources.Load<BoardGame>(path+ _currentLevel));
            }
        }
        

        [Button]
        private void JsonLoad()
        {
            string path = Application.dataPath + $"/Resources/DesignJson/Level{_currentLevel}.json";
            string levelTxt = File.ReadAllText(path);
            if(levelTxt.IsNullOrWhitespace()) return;
            
            BoardGame.TileJsonData tileJsonData = new BoardGame.TileJsonData();
            tileJsonData = JsonUtility.FromJson<BoardGame.TileJsonData>(levelTxt);
            
            if(tileJsonData == null) return;
            GameObject rootParent = new GameObject(tileJsonData.Level);
            JsonBoardGame boardGame = rootParent.AddComponent<JsonBoardGame>();
            _levelObject = boardGame;
            boardGame.Initialized(tileJsonData, tilePrefab, slotTransformPrefab);
        }

        public void AddTileToSlot(Tile tile)
        {
            if (ListSlots.Count < maxSlot)
            {
                int indexAdd = FindIndexToAdd(tile);
                TileSlot tileSlot = Instantiate(itemTileSlotPrefab, _levelObject.slotTransform);
                tileSlot.transform.localPosition = GameConfig.GetAddTile(indexAdd);
                tile.transform.SetParent(tileSlot.transform);

                tileSlot.SetTile(tile);
                tile.TweenPlayMoveToSlot(indexAdd);
                _levelObject.AddUndo(tileSlot);
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
                    _levelObject.RemoveUndo(slot);
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

        #region Undo

        public void OnButtonUndo()
        {
            if (GameState == GameState.PLAYING && _levelObject.CheckUndoAvailable())
            {
                if (GameConfig.UsePowerUp(PowerUpType.Undo)) _levelObject.SetUndo();
            }
        }

        #endregion

        #region Shuffle

        public void OnButtonShuffle()
        {
            if (GameState == GameState.PLAYING)
            {
                if (GameConfig.UsePowerUp(PowerUpType.Shuffle)) _levelObject.Shuffle();
            }
        }

        #endregion

        #region Suggest

        public void OnButtonSuggest()
        {
            if (GameState == GameState.PLAYING)
            {
                if (GameConfig.UsePowerUp(PowerUpType.Suggests)) _levelObject.Suggest();
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
            PlayerPrefs.SetInt(StringConstants.SAVE_LEVEL, _currentLevel);
            PlayerPrefs.Save();
        }

        private void StartLevel()
        {
            SceneManager.LoadScene(StringConstants.LOAD_LEVEL);
        }
        
        public void LoadNextLevel()
        {
            _currentLevel++;
            SaveLevel();
            SceneManager.LoadScene(StringConstants.LOAD_LEVEL);
            GCCollectAndClear();
        }

        public void ReloadLevelAt(int level)
        {
            _currentLevel = level;
            SaveLevel();
            SceneManager.LoadScene(StringConstants.LOAD_LEVEL);
            GCCollectAndClear();
        }


        public void RestartLevel()
        {
            SceneManager.LoadScene(StringConstants.LOAD_LEVEL);
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