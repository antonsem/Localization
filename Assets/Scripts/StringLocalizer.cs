using System;
using UnityEngine;
using TMPro;
using MyBox;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StringLocalizer : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private TextMeshProUGUI textField;
    [SerializeField]
    private Translation defaultString;

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

    [ButtonMethod]
    private void GetValueFromText()
    {
        if(!Enum.TryParse(textField.text, out Translation id))
        {
            Debug.LogError($"The Translation enum does not have a value of '<color=red>{textField.text}</color>'! Did you forgot to update it?");
            return;
        }

        defaultString = id;
    }

    private void Reset()
    {
        if (TryGetComponent(out textField))
            return;
        
        Debug.LogError($"{name} does not have a TextMeshProUGUI component. The StringLocalizer cannot work without one!", this);
        enabled = false;
    }
}