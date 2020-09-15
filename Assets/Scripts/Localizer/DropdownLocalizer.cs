using System;
using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TMP_Dropdown)), DisallowMultipleComponent]
    public class DropdownLocalizer : MonoBehaviour
    {
        [SerializeField, HideInInspector] private TMP_Dropdown dropdown;
        [SerializeField, HideInInspector] private TMP_Text label;
        [SerializeField, HideInInspector] private string[] defaultStrings;
        private Translation[] _enumValues;
        private Translation[] EnumValues
        {
            get
            {
                if (_enumValues == null || _enumValues.Length == 0)
                    GetEnumValues();

                return _enumValues;
            }
            set => _enumValues = value;
        }

        private void OnEnable()
        {
            Localizer.languageChanged += UpdateLanguage;
            UpdateLanguage();
        }

        private void OnDisable()
        {
            Localizer.languageChanged -= UpdateLanguage;
        }

        private void GetEnumValues()
        {
            if (defaultStrings == null || defaultStrings.Length == 0)
            {
                Debug.LogWarning($"Default strings on {name} are not set, check it out!", this);
                GetDefaultStrings();
            }

            EnumValues = new Translation[defaultStrings.Length];
            for (int i = 0; i < defaultStrings.Length; i++)
            {
                if (Enum.TryParse(defaultStrings[i], out Translation id))
                {
                    EnumValues[i] = id;
                    continue;
                }

                Debug.LogError(
                    $"The Translation enum does not have a value of '<color=red>{defaultStrings[i]}</color>'! Did you forgot to update it?");
            }
        }

        private void UpdateLanguage()
        {
            for (int i = 0; i < EnumValues.Length; i++)
                dropdown.options[i].text = Localizer.Get(EnumValues[i]);

            label.text = Localizer.Get(EnumValues[dropdown.value]);
        }

        private void GetDefaultStrings()
        {
            defaultStrings = new string[dropdown.options.Count];
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (Enum.TryParse(dropdown.options[i].text, out Translation _))
                {
                    defaultStrings[i] = dropdown.options[i].text;
                    continue;
                }

                Debug.LogError(
                    $"The Translation enum does not have a value of '<color=red>{dropdown.options[i].text}</color>'! Did you forgot to update it?");

                defaultStrings[i] = ((Translation) 0).ToString();
            }
        }

        private void Reset()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            if (dropdown.captionText)
                label = dropdown.captionText;
            GetDefaultStrings();
#if UNITY_EDITOR
            UnityEditor.EditorUtility.SetDirty(gameObject);
#endif
        }
    }
}