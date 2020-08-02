using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;

public static class Localizer
{
    /// <summary>
    /// Invoked when the language is changed
    /// </summary>
    public static Action languageChanged;

    private static Dictionary<Translation, TranslationString> translations;
    private static Dictionary<Translation, TranslationString> Translations
    {
        get
        {
            if (translations == null)
                ReadLocalizationFile($"{Application.streamingAssetsPath}/Localization.csv");

            return translations;
        }
    }

    public static string[] Languages { get; private set; }

    private static string currentLanguage = "English";

    public static string CurrentLanguage
    {
        get => currentLanguage;
        set
        {
            if (value == currentLanguage) return;
            currentLanguage = value;
            languageChanged?.Invoke();
        }
    }

    /// <summary>
    /// Reads a localization file at a given path
    /// </summary>
    /// <param name="path">Path of the csv file, including the file name</param>
    private static void ReadLocalizationFile(in string path)
    {
        if (!File.Exists(path)) return;

        string[] lines = File.ReadAllLines(path, System.Text.Encoding.UTF8); //Read lines
        int stringCount = lines.Length; //Cache translation string count
        translations =
            new Dictionary<Translation, TranslationString>(stringCount - 1); //Initialize translation dictionary

        string[] tempLanguages = lines[0].Split(';'); //Setup Languages
        Languages = new string[tempLanguages.Length - 1];
        for (int i = 0; i < tempLanguages.Length - 1; i++)
            Languages[i] = tempLanguages[i + 1];

        CurrentLanguage = Languages[0];

        int langCount = Languages.Length;
        string[] stringTranslations = new string[langCount]; //Temporary array to store translations for initialization

        for (int i = 1; i < stringCount; i++)
        {
            stringTranslations = lines[i].Split(';'); //Split the line to individual translated strings

            if (!Enum.TryParse(stringTranslations[0], out Translation id)) //Check if there is a corresponding enum value
            {
                Debug.LogError(
                    $"Translation enum does not contain a value of '{stringTranslations[0]}'! Did you forgot to update the Translations enum?");
                continue;
            }

            TranslationString temp = new TranslationString(langCount);
            for (int j = 0; j < langCount; j++)
            {
                temp.translationDict.Add(Languages[j],
                    stringTranslations.Length > j + 1 ? stringTranslations[j + 1].Trim('"') : stringTranslations[0]);
            }

            translations.Add(id, temp);
        }
    }

    /// <summary>
    /// Returns a localized string in current language
    /// </summary>
    /// <param name="id">String id</param>
    /// <returns></returns>
    public static string Get(in Translation id)
    {
        return Get(CurrentLanguage, id);
    }

    /// <summary>
    /// Returns localized string in a requested language
    /// </summary>
    /// <param name="language">Requested language</param>
    /// <param name="id">Requested string</param>
    public static string Get(in string language, in Translation id)
    {
        if (Translations.ContainsKey(id))
            return Translations[id].Get(language);

        Debug.LogErrorFormat("Translations does not contain a string with id '<color=red>{0}</color>'!", id);
        return $"{id} ({language})";
    }
}

public readonly struct TranslationString
{
    public readonly Dictionary<string, string> translationDict; //A Language - Translation dictionary

    public TranslationString(int languageCount) //Constructor
    {
        translationDict = new Dictionary<string, string>(languageCount);
    }

    /// <summary>
    /// Returns the translation of the string in a given language
    /// </summary>
    /// <param name="language">Language to translate the string to</param>
    public string Get(in string language)
    {
        if (translationDict.ContainsKey(language)) //Check if we have a translation for the requested language
            return translationDict[language];

        Debug.LogErrorFormat("Translation does not contain language '<color=red>{0}</color>'!", language);
        return $"N/A language '{language}'";
    }
}