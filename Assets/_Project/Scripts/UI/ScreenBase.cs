using UnityEngine;

namespace Match_3
{
    public class ScreenBase : MonoBehaviour
    {
        protected virtual void OnEnable()
        {
            GameManager.Current.GameState = GameState.PAUSE;
        }

        protected virtual void OnDisable()
        {
            GameManager.Current.GameState = GameState.PLAYING;
        }
    }
}
