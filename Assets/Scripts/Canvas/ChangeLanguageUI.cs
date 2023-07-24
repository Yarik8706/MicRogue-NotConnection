using System;
using MainScripts;
using TMPro;
using UnityEngine;

namespace Canvas
{
    public class ChangeLanguageUI : MonoBehaviour
    {
        [SerializeField] private TMP_Text ruSelect;
        [SerializeField] private TMP_Text enSelect;

        private void Awake()
        {
            TranslateText.language = PlayerPrefs.GetString("LANGUAGE", "EN");
            ChangeUI(PlayerPrefs.GetString("LANGUAGE"));
        }

        public void ChangeLanguage(string language)
        {
            ChangeUI(language);
            PlayerPrefs.SetString("LANGUAGE", language);
            TranslateText.language = language;
            UIEventManager.OnUpdateLanguage.Invoke();
        }

        private void ChangeUI(string language)
        {
            switch (language)
            {
                case "EN":
                    enSelect.fontStyle = FontStyles.Underline;
                    ruSelect.fontStyle = FontStyles.Normal;
                    break;
                case "RU":
                    ruSelect.fontStyle = FontStyles.Underline;
                    enSelect.fontStyle = FontStyles.Normal;
                    break;
            }
        }
    }
}