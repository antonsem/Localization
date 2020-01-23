using UnityEngine;
using TMPro;
using MyBox;

[RequireComponent(typeof(TextMeshProUGUI))]
public class StringLocalizer : MonoBehaviour
{
    [SerializeField, ReadOnly]
    private TextMeshProUGUI textField;
    [SerializeField, ReadOnly]
    private string defaultString = "";

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
        if (!string.IsNullOrEmpty(defaultString))
            textField.text = Localizer.Get(defaultString);
        else
            Debug.LogErrorFormat("The string of text object '{0}' is empty. Cannot set the language!", this);
    }

    [ButtonMethod]
    private void GetDefaultString()
    {
        defaultString = textField.text;
    }

    private void Reset()
    {
        textField = GetComponent<TextMeshProUGUI>();
        defaultString = textField.text;
    }
}