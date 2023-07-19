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
            DOTween.Kill(gameObject.transform);
            sequence = DOTween.Sequence();
            sequence.Insert(0f, gameObject.transform.DOLocalMove(new Vector3(-3 * 1.2f + indexSlot  * 1.2f, 0f, 0f), 0.2f).SetEase(Ease.OutQuad));
        }
    }
}