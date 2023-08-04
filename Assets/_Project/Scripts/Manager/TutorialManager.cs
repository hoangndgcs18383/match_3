using UnityEngine;

namespace Match_3
{
    public class TutorialManager : MonoBehaviour
    {
        public static TutorialManager Current;

        private GameObject _tutorialHandObject;

        public GameObject TutorialHandObject
        {
            get
            {
                if (_tutorialHandObject == null)
                {
                    _tutorialHandObject = Resources.Load<GameObject>("Prefabs/TutorialHand");
                }

                return _tutorialHandObject;
            }
            set => _tutorialHandObject = value;
        }

        private void Awake()
        {
            if (Current != null) Destroy(gameObject);
            else
            {
                Current = this;
                DontDestroyOnLoad(this);
            }
        }
    }
}