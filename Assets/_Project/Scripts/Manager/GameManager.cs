using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using Sirenix.OdinInspector;
using Sirenix.Utilities;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.U2D;
using Zeff.Extensions;
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

        private BoardGame _levelObject;

        public List<TileSlot> ListSlots { get; } = new List<TileSlot>();
        public List<TileDirection> ListDirections { get; set; } = new List<TileDirection>();
        public GameState GameState { get; set; }

        public int Level
        {
            get => ProfileDataService.Instance.ProfileData.Level;
            private set => ProfileDataService.Instance.SetLevel(value);
        }

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
            RewardManager.Current.UpdateCoinView();
            
            ProfileDataService.Instance.AutoSaveProfile();
        }


        #region Update Method

        private void Update()
        {
            TimingService.Instance.Update();
        }

        private void FixedUpdate()
        {
            TimingService.Instance.FixedUpdate();
        }

        private void LateUpdate()
        {
            TimingService.Instance.LateUpdate();
        }

        #endregion


        private void SetTargetFPS()
        {
            QualitySettings.vSyncCount = 0;
            Application.targetFrameRate = 60;
        }
        
        #region Board

        public void LoadLevel()
        {
            GameState = GameState.START;

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

            UIManager.Current.SetLevelText(Level);
        }

        private void ManualLoad(string path)
        {
            BoardGame boardGame = Resources.Load<BoardGame>(path + Level);

            if (boardGame != null)
            {
                _levelObject = Instantiate(boardGame);
                _levelObject.Initialized();
            }
            else
            {
                Level = Random.Range(0, 50);
                _levelObject = Instantiate(Resources.Load<BoardGame>(path + Level));
            }

            Debug.LogError("[Error] Counldn't load Json, [Load Manual Success]");
        }


        [Button]
        private void JsonLoad()
        {
            try
            {
                //string path = $"Resources/DesignJson/Level{_currentLevel}.json";

                string path = $"/LevelData/Level{Level}.bin";

                string levelTxt = JsonHelper.GetFileStream(path);

                if (levelTxt.IsNullOrWhitespace())
                {
                    LoadLevelType = LoadLevelType.ManualLoad;
                    LoadLevel();
                };

                BoardGame.TileJsonData tileJsonData = new BoardGame.TileJsonData();
                tileJsonData = JsonUtility.FromJson<BoardGame.TileJsonData>(levelTxt);

                if (tileJsonData == null) return;
                GameObject rootParent = new GameObject(tileJsonData.Level);
                JsonBoardGame boardGame = rootParent.AddComponent<JsonBoardGame>();
                _levelObject = boardGame;
                boardGame.Initialized(tileJsonData, tilePrefab, slotTransformPrefab);
            }
            catch (Exception e)
            {
                Debug.LogError($"[Error] {e}");
                LoadLevelType = LoadLevelType.ManualLoad;
                LoadLevel();
            }
        }
        
        private void OnNotEnoughCoin(PowerUpType powerUpType)
        {
            AdsManager.Current.ShowRewardedAd(() =>
            {
                RewardManager.Current.AddReward(powerUpType, 1);
            });
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
                //Check loseOnMoveComplete
                _levelObject.OnMoveComplete(ListSlots);
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
            if (GameState == GameState.PLAYING && _levelObject.CheckUndoAvailable() && !_levelObject.IsRunningPowerUp)
            {
                GameConfig.UsePowerUp(PowerUpType.Undo, () => { _levelObject.SetUndo(); }, OnNotEnoughCoin);
            }
        }

        #endregion

        #region Shuffle

        public void OnButtonShuffle()
        {
            if (GameState == GameState.PLAYING && !_levelObject.IsRunningPowerUp)
            {
                GameConfig.UsePowerUp(PowerUpType.Shuffle, () => { _levelObject.Shuffle(); }, OnNotEnoughCoin);
            }
        }

        #endregion

        #region Suggest

        public void OnButtonSuggest()
        {
            if (GameState == GameState.PLAYING && !_levelObject.IsRunningPowerUp)
            {
                GameConfig.UsePowerUp(PowerUpType.Suggests, () => { _levelObject.Suggest(); }, OnNotEnoughCoin);
            }
        }

        #endregion

        #endregion

        #region Level

        public void SetLevel(int level)
        {
            Level = level;
            SaveLevel();
        }

        private void SaveLevel()
        {
        }

        private void StartLevel()
        {
            SceneManager.LoadScene(StringConstants.LOAD_LEVEL);
        }

        public void LoadNextLevel()
        {
            ProfileDataService.Instance.UpdateNextLevel();
            ClearLevel();
            LoadingManager.Instance.LoadScene(StringConstants.LOAD_LEVEL);
            GCCollectAndClear();
        }

        public void ReloadLevelAt(int level)
        {
            Level = level;

            ClearLevel();
            SaveLevel();
            LoadingManager.Instance.LoadScene(StringConstants.LOAD_LEVEL);
            GCCollectAndClear();
        }


        public void RestartLevel()
        {
            ClearLevel();
            LoadingManager.Instance.LoadScene(StringConstants.LOAD_LEVEL);
            GCCollectAndClear();
        }

        private void ClearLevel()
        {
            ListSlots.Clear();
            ListDirections.Clear();
            GameState = GameState.END;
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

        private void OnApplicationQuit()
        {
            TimerManager.Current.StopCountDown();
            ProfileDataService.Instance.SaveProfileData();
        }

        #endregion
    }
}