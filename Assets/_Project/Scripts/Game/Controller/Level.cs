using UnityEngine;

namespace Match_3
{
    public class Level : MonoBehaviour
    {
        void Start()
        {
            GameManager.Current.LoadLevel();
        }
    }
}
