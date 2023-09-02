using System;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace Zeff.Core.Localization
{
    public class LocalizationTMPText : MonoBehaviour
    {
        public string Key;
        
        [HideInInspector] public TMP_Text targetText;
        
        private string _key;

        private void Start()
        {
            if (targetText == null)
            {
                targetText = GetComponent<TMP_Text>();
            }
            
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
            
            if (targetText == null)
            {
                Debug.LogError("LocalizationTMPText: targetText is null");
            }
        }

        private void OnSelectedLocaleChanged(Locale obj)
        {
            SetText(Key);
        }

        public void SetText(string key)
        {
            try
            {
                string localizedText = LocalizationSettings.StringDatabase.GetLocalizedString(key);
                
                targetText.SetText(localizedText);
            }
            catch (Exception e)
            {
                Debug.LogError("LocalizationTMPText: " + e.Message);
            }
        }
    }
}
