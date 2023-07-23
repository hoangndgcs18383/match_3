using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
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
        public static float TILE_SIZE = 1.2f;

        [SerializeField] private SpriteRenderer bg;
        [SerializeField] private SpriteRenderer icon;
        [SerializeField] private SpriteRenderer shadow;
        [SerializeField] private GameObject tileObject;

        [SerializeField] private TileCollider tileCollider;

        private Collider2D _touchCollider;
        public TileData data;
        public TileState tileState;

        private float _z;

        private void Awake()
        {
            _touchCollider = GetComponent<Collider2D>();
            _touchCollider.enabled = true;

            tileState = TileState.START;
            tileCollider.gameObject.SetActive(false);
        }

        public void Initialized(TileData data)
        {
            this.data = data;

            _z = GetZ(800, this.data.IndexOnMap);
            //load sprite
            icon.sprite = Resources.Load<Sprite>($"Sprites/{(int)data.ItemData.tileType}");
            //set local position
            SetPos();
            SetOrderLayerFloor();
            SetLayerFloor();
            ShowTile();
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
            bg.sortingLayerName = "MOVE";
            icon.sortingLayerName = "MOVE";
            shadow.sortingLayerName = "MOVE";
            
            tileCollider.gameObject.transform.localPosition = new Vector3(0f, 0f, 0.5f);
        }

        private void ShowTile()
        {
            tileState = TileState.START_TO_FLOOR;

            List<TileDirection> listDirections = GameManager.Current.ListDirections;

            switch (listDirections[data.FloorIndex])
            {
                case TileDirection.TOP:
                    transform.localPosition = MathExtenstion.TileToTop(TILE_SIZE, data.PosTile, data.FloorIndex);
                    break;
                case TileDirection.BOTTOM:
                    transform.localPosition = MathExtenstion.TileToBottom(TILE_SIZE, data.PosTile, data.FloorIndex);
                    break;
                case TileDirection.LEFT:
                    transform.localPosition = MathExtenstion.TileToLeft(TILE_SIZE, data.PosTile, data.FloorIndex);
                    break;
                case TileDirection.RIGHT:
                    transform.localPosition = MathExtenstion.TileToRight(TILE_SIZE, data.PosTile, data.FloorIndex);
                    break;
            }

            transform.DOLocalMove(new Vector3(TILE_SIZE * data.PosTile.x, TILE_SIZE * data.PosTile.y, _z), 1f)
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
            StartCoroutine(IESetTouchEnable());
        }

        private IEnumerator IESetTouchEnable()
        {
            yield return new WaitForSeconds(0.1f);
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

        public void ResetPosSlot(int indexSlot)
        {
            SetLayersToMoveSlot(indexSlot);
        }

        private void SetPos()
        {
            //Match
            transform.localPosition = new Vector3(TILE_SIZE * data.PosTile.x, TILE_SIZE * data.PosTile.y,
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

        private Sequence _moveToSlotSequence;

        public void TweenPlayMoveToSlot(int indexSlot)
        {
            DOTween.Kill(gameObject.transform);

            tileCollider.gameObject.SetActive(false);
            SetLayersToMoveSlot(indexSlot);
            SetLayerDefault();
            
            _moveToSlotSequence = DOTween.Sequence();
            _moveToSlotSequence.Insert(0f,
                tileObject.transform.DOScale(Vector3.one * 1.1f, 0.1f).SetEase(Ease.OutQuad));
            _moveToSlotSequence.Insert(0f,
                tileObject.transform.DOLocalRotate(new Vector3(0f, 0f, Random.Range(10f, 15f)), 0.1f));
            _moveToSlotSequence.Insert(0.1f, tileObject.transform.DOScale(Vector3.one, 0.2f).SetEase(Ease.InQuad));
            _moveToSlotSequence.Insert(0.1f,
                tileObject.transform.DOLocalRotate(Vector3.zero, 0.2f).SetEase(Ease.InQuad));
            _moveToSlotSequence.Insert(0f, gameObject.transform.DOLocalMove(Vector3.zero, 0.3f).SetEase(Ease.OutQuad));
            _moveToSlotSequence.OnComplete(() =>
            {
                tileState = TileState.SLOT;
                GameManager.Current.OnCompleteMoveToSlot(this);
            });
        }

        #region WINDOW_WEB

        private void OnMouseDown()
        {
            OnTouchTile();
        }

        #endregion
    }
}