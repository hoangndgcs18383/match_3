using System;
using DG.Tweening;
using UnityEngine;

namespace Match_3
{
    public class TileSlot : MonoBehaviour
    {
        private Tile _tile;

        public Tile Tile => _tile;

        public void SetTile(Tile tile)
        {
            _tile = tile;
        }
        
        Sequence sequence;
        
        public void ResetPosSlot(int indexSlot)
        {
            _tile.ResetPosSlot(indexSlot);
            DOTween.Kill(transform);
            sequence = DOTween.Sequence();
            sequence.Insert(0f, transform.DOLocalMove(GameConfig.GetMoveTile(indexSlot), 0.2f).SetEase(Ease.OutQuad));
        }

        public void SetMatchCallback()
        {
            _tile.SetMatch(Hide);
        }

        private void Hide()
        {
            gameObject.SetActive(false);
        }
    }
}