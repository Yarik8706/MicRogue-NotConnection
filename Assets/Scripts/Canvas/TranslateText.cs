using System;
using MainScripts;
using TMPro;
using UnityEngine;

namespace Canvas
{
    public class TranslateText : MonoBehaviour
    {
        private TMP_Text _text;

        public static string language = "EN";

        [SerializeField] private string englishText;
        [SerializeField] private string russianText;

        private void Awake()
        {
            _text = GetComponent<TMP_Text>();
            UIEventManager.OnUpdateLanguage.AddListener(UpdateText);
        }

        private void Start()
        {
            UpdateText();
        }

        private void OnEnable()
        {
            UpdateText();
        }

        private void UpdateText()
        {
            _text.text = language == "EN" ? englishText : russianText;
        }
    }
}