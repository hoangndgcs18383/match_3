using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace Match_3
{
    public class LocalizationManager : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown _dropdown;
        
        private IEnumerator Start()
        {
            yield return LocalizationSettings.InitializationOperation;
            
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.GetLocale(PlayerPrefs.GetString("Language", "en"));
            Debug.Log("LocalizationManager: " + LocalizationSettings.SelectedLocale.name);

            List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
            int selected = 0;
            for (int i = 0; i < LocalizationSettings.AvailableLocales.Locales.Count; i++)
            {
                Locale locale = LocalizationSettings.AvailableLocales.Locales[i];
                if (LocalizationSettings.SelectedLocale == locale) selected = i;
                options.Add(new TMP_Dropdown.OptionData(locale.name));
            }
            
            
            
            _dropdown.options = options;
            _dropdown.value = selected;
            _dropdown.onValueChanged.AddListener(OnValueChanged);
        }

        private void OnValueChanged(int arg0)
        {
            LocalizationSettings.SelectedLocale = LocalizationSettings.AvailableLocales.Locales[arg0];
            //Set To Local
            PlayerPrefs.SetString("Language", LocalizationSettings.SelectedLocale.Identifier.Code);
            PlayerPrefs.Save();
        }
    }
}