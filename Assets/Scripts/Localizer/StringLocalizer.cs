using System;
using UnityEngine;
using TMPro;

namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI)), DisallowMultipleComponent]
    public class StringLocalizer : MonoBehaviour
    {
        [SerializeField, HideInInspector] private TextMeshProUGUI textField;
        [SerializeField] private Translation defaultString;

        private void OnEnable()
        {
            Localizer.languageChanged += UpdateLanguage;
            UpdateLanguage();
        }

        private void OnDisable()
        {
            Localizer.languageChanged -= UpdateLanguage;
        }

        private void UpdateLanguage()
        {
            textField.text = Localizer.Get(defaultString);
        }

        private void GetValueFromText()
        {
            if (!Enum.TryParse(textField.text, out Translation id))
            {
                Debug.LogError(
                    $"The Translation enum does not have a value of '<color=red>{textField.text}</color>'! Did you forgot to update it?");
                return;
            }

            defaultString = id;
        }

        private void Reset()
        {
            textField = GetComponent<TextMeshProUGUI>();
            GetValueFromText();
        }
    }
}