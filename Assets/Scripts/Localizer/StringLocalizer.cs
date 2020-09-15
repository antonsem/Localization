using System;
using UnityEngine;
using TMPro;

namespace Localization
{
    [RequireComponent(typeof(TextMeshProUGUI)), DisallowMultipleComponent]
    public class StringLocalizer : MonoBehaviour
    {
        [SerializeField, HideInInspector] private TextMeshProUGUI textField;
        [SerializeField] private string defaultString;
        
        private Translation enumValue;
        private Translation EnumValue
        {
            get
            {
                if(!_isSet)
                    GetEnum();
                
                return enumValue;
            }
        }

        private bool _isSet = false;

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
            textField.text = Localizer.Get(EnumValue);
        }

        private void GetEnum()
        {
            if (!Enum.TryParse(defaultString, out enumValue))
            {
                Debug.LogError(
                    $"The Translation enum does not have a value of '<color=red>{textField.text}</color>'! Did you forgot to update it?");
                return;
            }

            _isSet = true;
        }
        
        private void GetValueFromText()
        {
            if (!Enum.TryParse(textField.text, out Translation _))
            {
                Debug.LogError(
                    $"The Translation enum does not have a value of '<color=red>{textField.text}</color>'! Did you forgot to update it?");
                defaultString = ((Translation) 0).ToString();
                return;
            }

            defaultString = textField.text;
        }

        private void Reset()
        {
            textField = GetComponent<TextMeshProUGUI>();
            GetValueFromText();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }
    }
}