using System;
using UnityEngine;

namespace Match_3
{
    public class TileCollider : MonoBehaviour
    {
        public Tile tileParent;

        private int _countCollision;

        private void Start()
        {
            _countCollision = 0;
        }

        public void SetAvailable()
        {
            transform.localPosition = new Vector3(0f, 0f, 0.5f);
        }

        private void OnTriggerEnter2D(Collider2D collider)
        {
            if (collider.CompareTag("CheckCollider"))
            {
                if (tileParent.tileState == TileState.FLOOR)
                {
                    if (collider.GetComponent<TileCollider>().tileParent.data.FloorIndex > tileParent.data.FloorIndex)
                    {
                        _countCollision++;
                        tileParent.SetTouchAvailable(false);
                        tileParent.SetShadowAvailable(true);
                    }
                }
            }
        }

        private void OnTriggerExit2D(Collider2D collider)
            {
                if (collider.CompareTag("CheckCollider"))
                {
                    if (tileParent.tileState == TileState.FLOOR)
                    {
                        if (collider.GetComponent<TileCollider>().tileParent.data.FloorIndex > tileParent.data.FloorIndex)
                        {
                            _countCollision--;
                        }

                        if (_countCollision <= 0) tileParent.SetTouchEnable();
                    }
                }
            }
        }
    }