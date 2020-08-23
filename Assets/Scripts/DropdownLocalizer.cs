using System;
using TMPro;
using UnityEngine;

namespace Localization
{
    [RequireComponent(typeof(TMP_Dropdown)), DisallowMultipleComponent]
    public class DropdownLocalizer : MonoBehaviour
    {
        [SerializeField] private TMP_Dropdown dropdown;
        [SerializeField] private TextMeshProUGUI label;
        [SerializeField] private Translation[] defaultStrings;

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
            for (int i = 0; i < defaultStrings.Length; i++)
                dropdown.options[i].text = Localizer.Get(defaultStrings[i]);

            label.text = Localizer.Get(defaultStrings[dropdown.value]);
        }

        [MyBox.ButtonMethod]
        private void GetDefaultStrings()
        {
            defaultStrings = new Translation[dropdown.options.Count];
            for (int i = 0; i < dropdown.options.Count; i++)
            {
                if (!Enum.TryParse(dropdown.options[i].text, out Translation id))
                {
                    Debug.LogError(
                        $"The Translation enum does not have a value of '<color=red>{dropdown.options[i].text}</color>'! Did you forgot to update it?");
                    continue;
                }

                defaultStrings[i] = id;
            }
        }

        private void Reset()
        {
            dropdown = GetComponent<TMP_Dropdown>();
            GetDefaultStrings();
        }
    }
}