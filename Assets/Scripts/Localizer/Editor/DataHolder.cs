using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Localization
{
    public class DataHolder
    {
        private static readonly string[] _defaultCSV = new string[] {"id;English", "UI_hello;Hello World"};

        public DataHolder(in string path)
        {
            TranslationStrings = ReadLocalizationFile(path);
        }

        public List<ETS> TranslationStrings { get; }

        public List<string> Languages { get; private set; }

        private List<ETS> ReadLocalizationFile(in string path)
        {
            return !File.Exists(path)
                ? LoadFromCSV(_defaultCSV)
                : LoadFromCSV(File.ReadAllLines(path, System.Text.Encoding.UTF8));
        }

        private List<ETS> LoadFromCSV(in string[] lines)
        {
            int stringCount = lines.Length; //Cache translation string count
            List<ETS> translations =
                new List<ETS>(stringCount - 1); //Initialize translation dictionary

            Languages = new List<string>(lines[0].Split(';'));
            Languages.RemoveAt(0);

            for (int i = 1; i < stringCount; i++)
            {
                IgnoreEscapeChar(out List<string> stringTranslations, lines[i].Split(';').ToList()); //Split the line to individual translated strings
                string id = stringTranslations[0];
                stringTranslations.RemoveAt(0);
                ETS ets = new ETS(id, Languages.ToList(), stringTranslations);
                translations.Add(ets);
            }

            return translations;
        }
        
        private static void IgnoreEscapeChar(out List<string> combinedLines, IReadOnlyList<string> lines)
        {
            combinedLines = new List<string>(lines.Count);
            for (int i = 0; i < lines.Count; i++)
            {
                string line = lines[i];
                while (line[line.Length - 1] == '\\' && (line.Length < 2 || line[line.Length - 2] != '\\'))
                {
                    if (++i >= lines.Count) break;
                    line = $"{line.Remove(line.Length - 1)};{lines[i]}";
                }

                combinedLines.Add(line);
            }
        }

        public bool HasNonUniqueKeys()
        {
            bool retVal = false;

            foreach (ETS ets in TranslationStrings)
                ets.IsKeyUnique = true;

            for (int i = 0; i < TranslationStrings.Count; i++)
            {
                if (!TranslationStrings[i].IsKeyUnique) continue;
                for (int j = i + 1; j < TranslationStrings.Count; j++)
                {
                    if (TranslationStrings[i].Key != TranslationStrings[j].Key) continue;
                    TranslationStrings[i].IsKeyUnique = false;
                    TranslationStrings[j].IsKeyUnique = false;
                    retVal = true;
                    break;
                }
            }

            return retVal;
        }

        public void AddLanguage(string newLanguage)
        {
            while (Languages.Contains(newLanguage))
                newLanguage = $"{newLanguage} (new)";

            Languages.Add(newLanguage);

            foreach (ETS ets in TranslationStrings)
                ets.AddLanguage(newLanguage);
        }

        public void RenameLanguage(in string oldLanguage, in string newLanguage)
        {
            int index = Languages.IndexOf(oldLanguage);
            if (index < 0)
            {
                Debug.Log($"No language {oldLanguage}!");
                return;
            }

            Languages[index] = newLanguage;
            foreach (ETS ets in TranslationStrings)
                ets.RenameLanguage(oldLanguage, newLanguage);
        }

        public void RemoveLanguage(in string language)
        {
            Languages.Remove(language);
            foreach (ETS ets in TranslationStrings)
                ets.RemoveLanguage(language);
        }
    }

    public class ETS
    {
        private static readonly Regex _rgx = new Regex("[^a-zA-Z0-9]");
        public string Key { get; private set; }
        private string _defaultKey;
        public List<string> Languages { get; }
        public List<string> Translations { get; private set; }
        private List<string> _defaultTranslations;
        public bool IsDeleted { get; set; } = false;
        public List<bool> IsEdited { get; }
        public bool IsKeyEdited { get; private set; } = false;
        public bool IsKeyUnique { get; set; } = true;
        public bool IsNew { get; private set; } = false;

        public ETS(in string key, List<string> languages, List<string> translations, bool isNew = false)
        {
            _defaultKey = Key = key;
            Languages = languages;
            Translations = translations;
            _defaultTranslations = new List<string>(translations);
            IsNew = isNew;
            IsEdited = new List<bool>(Translations.Count);
            foreach (string _ in Languages)
                IsEdited.Add(false);
        }

        public void SetKey(string key)
        {
            key = _rgx.Replace(key, "_");
            if (key.Length > 0 && int.TryParse(key[0].ToString(), out int _))
                key = key.Insert(0, "_");

            Key = key;
            IsKeyEdited = true;
        }

        public string Get(in string language)
        {
            for (int i = 0; i < Languages.Count; i++)
            {
                if (Languages[i] == language)
                    return Translations[i];
            }

            return $"No translation for {Key} in {language}";
        }

        public void Set(in string language, string val)
        {
            for (int i = 0; i < Languages.Count; i++)
            {
                if (Languages[i] != language) continue;
                Translations[i] = val;
                IsEdited[i] = true;
                return;
            }

            Debug.LogError($"Couldn't find language {language}!");
        }

        public void RenameLanguage(in string oldLanguage, in string newLanguage)
        {
            for (int i = 0; i < Languages.Count; i++)
            {
                if (Languages[i] != oldLanguage) continue;
                Languages[i] = newLanguage;
                return;
            }

            Debug.LogError($"Couldn't find language {oldLanguage}!");
        }

        public void AddLanguage(string language)
        {
            Languages.Add(language);
            Translations.Add(Key);
            IsEdited.Add(false);
        }

        public void ResetAll()
        {
            Key = _defaultKey;
            Translations = _defaultTranslations;
            IsDeleted = false;
            IsKeyEdited = false;
            for (int i = 0; i < IsEdited.Count; i++)
                IsEdited[i] = false;
        }

        public void ResetTranslation(in string language)
        {
            for (int i = 0; i < Languages.Count; i++)
            {
                if (language != Languages[i]) continue;
                Translations[i] = _defaultTranslations[i];
                IsEdited[i] = false;
            }
        }

        public void ResetKey()
        {
            Key = _defaultKey;
            IsKeyEdited = false;
        }

        public void RemoveLanguage(in string language)
        {
            int index = Languages.IndexOf(language);
            if (index < 0)
            {
                Debug.LogError($"No {language} language!");
                return;
            }

            Languages.RemoveAt(index);
            Translations.RemoveAt(index);
            IsEdited.RemoveAt(index);
        }
    }
}