using UnityEngine;

namespace Match_3
{
    public class TileCollider : MonoBehaviour
    {
        public Tile tileParent;

        public int _countCollision;

        private void Start()
        {
            _countCollision = 0;
        }

        public void SetAvailable()
        {
            transform.localPosition = new Vector3(0f, 0f, 0.5f);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag("CheckCollider"))
            {
                if (tileParent.tileState == TileState.FLOOR)
                {
                    if (collision.GetComponent<TileCollider>().tileParent.data.FloorIndex > tileParent.data.FloorIndex)
                    {
                        _countCollision++;
                        tileParent.SetTouchAvailable(false);
                        tileParent.SetShadowAvailable(true);
                    }
                }
            }
        }

        private void OnTriggerStay2D(Collider2D collision)
        {
            if (collision.CompareTag("CheckCollider"))
            {
                if (tileParent.tileState == TileState.FLOOR)
                {
                    if (_countCollision <= 0) tileParent.SetTouchEnable();
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag("CheckCollider"))
            {
                if (tileParent.tileState == TileState.FLOOR)
                {
                    if (collision.GetComponent<TileCollider>().tileParent.data.FloorIndex > tileParent.data.FloorIndex)
                    {
                        _countCollision--;
                    }

                    if (_countCollision <= 0) tileParent.SetTouchEnable();
                }
            }
        }
    }
}