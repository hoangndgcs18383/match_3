using TMPro;
using UnityEngine;
using UnityEngine.Localization;

namespace Zeff.Core.Localization
{
    [RequireComponent(typeof(TMP_Text))]
    public class LocalizationTMPText : MonoBehaviour
    {
        [SerializeField] private LocalizedString localizedString;


        [HideInInspector] public TMP_Text targetTMPText;

        private int _argument;

        private void Awake()
        {
            if (targetTMPText == null)
                targetTMPText = GetComponent<TMP_Text>();
        }

        private void OnEnable()
        {
            if (localizedString == null)
            {
                Debug.LogError("Missing LocalizedString for " + gameObject.name);
                return;
            }

            localizedString.Arguments = new object[] {_argument};
            localizedString.StringChanged += OnStringChanged;
        }

        private void OnDisable()
        {
            if (localizedString == null)
            {
                Debug.LogError("Missing LocalizedString for " + gameObject.name);
                return;
            }

            localizedString.StringChanged -= OnStringChanged;
        }

        private void OnStringChanged(string value)
        {
            targetTMPText.SetText(value);
        }

        public void SetText(string value)
        {
            localizedString.GetLocalizedString(value);
            localizedString.RefreshString();
        }
        
        public void SetParams(params object[] arguments)
        {
            localizedString.Arguments = arguments;
            localizedString.RefreshString();
        }
    }
}