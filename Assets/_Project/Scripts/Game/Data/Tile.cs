using System;
using System.Collections.Generic;
using DG.Tweening;
using MEC;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Match_3
{
    [Serializable]
    public class TileData
    {
        private int _floorIndex;
        private Vector2Int _posTile;
        private int _indexOnMap;
        private ItemData _itemData;

        public int FloorIndex
        {
            get => _floorIndex;
            set => _floorIndex = value;
        }

        public Vector2Int PosTile
        {
            get => _posTile;
            set => _posTile = value;
        }

        public int IndexOnMap
        {
            get => _indexOnMap;
            set => _indexOnMap = value;
        }

        public ItemData ItemData
        {
            get => _itemData;
            set => _itemData = value;
        }

        public TileData(int floorIndex, int indexOnMap, Vector2Int posTile)
        {
            _floorIndex = floorIndex;
            _indexOnMap = indexOnMap;
            _posTile = posTile;
        }
    }

    public class Tile : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer bg;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private SpriteRenderer shadow;
        [SerializeField] private GameObject tileObject;

        [SerializeField] private TileCollider tileCollider;

        private Collider2D _touchCollider;
        public TileData data;
        public TileState tileState;
        private Vector3 _originalScale;

        private float _z;

        private void Awake()
        {
            _touchCollider = GetComponent<Collider2D>();
            _touchCollider.enabled = true;
            _originalScale = transform.localScale;

            tileState = TileState.START;
            tileCollider.gameObject.SetActive(false);
        }

        public void Initialized(TileData data)
        {
            this.data = data;

            _z = GetZ(800, this.data.IndexOnMap);
            //load sprite
            SetSprite();
            //set local position
            SetPos();
            SetOrderLayerFloor();
            SetLayerFloor();
            ShowTile();
        }

        public bool CanTouch()
        {
            return tileCollider.enabled;
        }

        private void SetSprite()
        {
            //icon.sprite = Resources.Load<Sprite>($"Sprites/{(int)data.ItemData.tileType}");
            icon.sprite = GameManager.Current.spriteTiles.GetSprite(((int)data.ItemData.tileType).ToString());
        }

        private void SetLayerFloor()
        {
            string sortingLayerName = "FLOOR_" + (data.FloorIndex + 1);
            bg.sortingLayerName = sortingLayerName;
            icon.sortingLayerName = sortingLayerName;
            shadow.sortingLayerName = sortingLayerName;

            tileCollider.transform.localPosition = new Vector3(0f, 0f, 0.5f);
        }

        private void SetOrderLayerFloor()
        {
            int sortingOrder = 400 - 20 * data.PosTile.y + data.PosTile.x;
            bg.sortingOrder = sortingOrder;
            icon.sortingOrder = sortingOrder;
            shadow.sortingOrder = sortingOrder;
        }

        private void SetLayerDefault()
        {
            string moveLayer = "MOVE";
            bg.sortingLayerName = moveLayer;
            icon.sortingLayerName = moveLayer;
            shadow.sortingLayerName = moveLayer;

            tileCollider.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
        }

        private void ShowTile()
        {
            tileState = TileState.START_TO_FLOOR;

            List<TileDirection> listDirections = GameManager.Current.ListDirections;

            switch (listDirections[data.FloorIndex])
            {
                case TileDirection.TOP:
                    transform.localPosition =
                        MathExtenstion.TileToTop(GameConfig.TILE_SIZE, data.PosTile, data.FloorIndex);
                    break;
                case TileDirection.BOTTOM:
                    transform.localPosition =
                        MathExtenstion.TileToBottom(GameConfig.TILE_SIZE, data.PosTile, data.FloorIndex);
                    break;
                case TileDirection.LEFT:
                    transform.localPosition =
                        MathExtenstion.TileToLeft(GameConfig.TILE_SIZE, data.PosTile, data.FloorIndex);
                    break;
                case TileDirection.RIGHT:
                    transform.localPosition =
                        MathExtenstion.TileToRight(GameConfig.TILE_SIZE, data.PosTile, data.FloorIndex);
                    break;
            }

            transform.DOLocalMove(
                    new Vector3(GameConfig.TILE_SIZE * data.PosTile.x, GameConfig.TILE_SIZE * data.PosTile.y, _z), 1f)
                .SetEase(Ease.InBack).SetDelay(0.04f * data.FloorIndex).OnComplete(() =>
                {
                    tileState = TileState.FLOOR;
                    tileCollider.gameObject.SetActive(true);
                });
        }

        public void OnTouchTile()
        {
            if (tileState == TileState.FLOOR)
            {
                // move to slot
                //Debug.Log("OnTouchTile");
                tileState = TileState.MOVE_TO_SLOT;
                SetTouchAvailable(false);
                SetShadowAvailable(false);
                GameManager.Current.AddTileToSlot(this);
            }
        }

        public void SetTouchEnable()
        {
            SetTouchAvailable(true);
            SetShadowAvailable(false);
        }

        private IEnumerator<float> IESetTouchEnable()
        {
            yield return Timing.WaitForSeconds(0.1f);
            SetTouchAvailable(true);
            SetShadowAvailable(false);
            //Debug.Log("SetTouchEnable");
        }

        public void SetTouchAvailable(bool isAvailable)
        {
            tileCollider.SetAvailable();
            _touchCollider.enabled = isAvailable;
        }

        public void SetShadowAvailable(bool b)
        {
            shadow.gameObject.SetActive(b);
        }

        public void SetSuggest()
        {
            tileCollider.gameObject.SetActive(false);
            SetLayerDefault();
            SetTouchAvailable(false);
            SetShadowAvailable(false);
            tileState = TileState.MOVE_TO_SLOT;

            GameManager.Current.AddTileToSlot(this);
        }

        public void ResetPosSlot(int indexSlot)
        {
            SetLayersToMoveSlot(indexSlot);
        }

        private void SetPos()
        {
            //Match
            transform.localPosition = new Vector3(GameConfig.TILE_SIZE * data.PosTile.x,
                GameConfig.TILE_SIZE * data.PosTile.y,
                50 - data.FloorIndex * 5);
        }

        private float GetZ(int value, int indexOnMap)
        {
            return value - indexOnMap;
        }


        private void SetLayersToMoveSlot(int indexSlot)
        {
            int sortingOrder = 10 * indexSlot;
            bg.sortingOrder = sortingOrder;
            icon.sortingOrder = sortingOrder;
            shadow.sortingOrder = sortingOrder;
        }

        #region Tweener

        private Sequence _matchSequence;

        public void SetMatch(Action onActionComplete)
        {
            float startTiming = 0;
            float nextTiming = 0.1f;
            
            _matchSequence = DOTween.Sequence();
            _matchSequence.Insert(startTiming, tileObject.transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutQuad));
            _matchSequence.Insert(startTiming,
                tileObject.transform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(10f, 15f)), 0.1f));
            _matchSequence.Insert(nextTiming, tileObject.transform.DOScale(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
            _matchSequence.Insert(nextTiming, tileObject.transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
            _matchSequence.OnComplete(() => { onActionComplete?.Invoke(); });
        }

        private Sequence _moveToSlotSequence;

        public void TweenPlayMoveToSlot(int indexSlot)
        {
            DOTween.Kill(gameObject.transform);

            float startTiming = 0;
            float nextTiming = 0.1f;
            
            //tileCollider.gameObject.SetActive(false);
            SetLayersToMoveSlot(indexSlot);
            SetLayerDefault();

            _moveToSlotSequence = DOTween.Sequence();
            _moveToSlotSequence.Insert(startTiming,
                tileObject.transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutQuad));
            _moveToSlotSequence.Insert(startTiming, transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.OutQuad));
            _moveToSlotSequence.Insert(startTiming,
                tileObject.transform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(10f, 15f)), 0.1f));
            _moveToSlotSequence.Insert(nextTiming, tileObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InQuad));
            _moveToSlotSequence.Insert(nextTiming,
                tileObject.transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
            _moveToSlotSequence.InsertCallback(nextTiming, () =>
            {
                // DO SOMETHING
                SoundManager.Current.PlaySound();
            });
            _moveToSlotSequence.OnComplete(() =>
            {
                tileState = TileState.SLOT;
                GameManager.Current.OnCompleteMoveToSlot(this);
            });
        }

        Sequence _shuffleSequence;

        public void SetShuffle(ItemData itemData)
        {
            data.ItemData = itemData;

            float startTiming = 0f;
            float nextTiming = 0.1f;

            _shuffleSequence = DOTween.Sequence();
            _shuffleSequence.Insert(startTiming, tileObject.transform.DOScale(Vector3.zero, 0.1f).SetEase(Ease.OutQuad));
            _shuffleSequence.Insert(startTiming,
                tileObject.transform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(10f, 15f)), 0.1f));
            _shuffleSequence.InsertCallback(nextTiming, SetSprite);
            _shuffleSequence.Insert(nextTiming, tileObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InQuad));
            _shuffleSequence.Insert(nextTiming, tileObject.transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
            _shuffleSequence.OnComplete(() =>
            {
                SetLayerFloor();
                tileCollider.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
            });
        }

        Sequence _undoSequence;

        public void SetUndo()
        {
            DOTween.Kill(gameObject.transform);

            SetLayersToMoveSlot(0);
            SetLayerDefault();

            float startTiming = 0;
            float nextTiming = 0.05f;

            _undoSequence = DOTween.Sequence();
            _undoSequence.Insert(startTiming, tileObject.transform.DOScale(Vector3.one * 1.1f, 0.05f).SetEase(Ease.OutQuad));
            _undoSequence.Insert(startTiming,
                tileObject.transform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(10f, 15f)), 0.05f));
            _undoSequence.Insert(startTiming, transform.DOScale(_originalScale, 0.05f).SetEase(Ease.InQuad));
            _undoSequence.Insert(startTiming,
                gameObject.transform
                    .DOLocalMove(
                        new Vector3(GameConfig.TILE_SIZE * data.PosTile.x, GameConfig.TILE_SIZE * data.PosTile.y, _z),
                        0.2f).SetEase(Ease.OutQuad));
            _undoSequence.Insert(nextTiming, tileObject.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InQuad));
            _undoSequence.Insert(nextTiming, tileObject.transform.DOLocalRotate(Vector3.zero, 0.1f).SetEase(Ease.InQuad));
            _undoSequence.OnComplete(OnUndoComplete);
        }

        public void SetUndoNow()
        {
            tileState = TileState.FLOOR;
            OnUndoComplete();
        }

        private void OnUndoComplete()
        {
            tileState = TileState.FLOOR;
            SetTouchAvailable(true);
            SetOrderLayerFloor();
            SetLayerFloor();
            tileCollider.gameObject.SetActive(true);
        }

        #endregion


        #region WINDOW_WEB

        private void OnMouseDown()
        {
            OnTouchTile();
        }

        private void OnMouseEnter()
        {
            tileObject.transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutQuad);
        }

        private void OnMouseExit()
        {
            tileObject.transform.DOScale(Vector3.one, 0.1f).SetEase(Ease.InQuad);
        }

        #endregion

        private void OnDestroy()
        {
            _shuffleSequence.Kill();
            _moveToSlotSequence.Kill();
            tileObject.transform.DOKill();
            _matchSequence.Kill();
        }
    }
}