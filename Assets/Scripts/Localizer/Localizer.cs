using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System;
using System.Linq;

namespace Localization
{
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

		/// <summary>
		/// Currently loaded languages
		/// </summary>
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
		private static void ReadLocalizationFile(string path)
		{
			if (!File.Exists(path)) return;

			//Read lines
			string[] lines = File.ReadAllLines(path, System.Text.Encoding.UTF8);
			//Cache translation string count
			int stringCount = lines.Length;
			//Initialize translation dictionary
			translations = new Dictionary<Translation, TranslationString>(stringCount - 1); 

			//Setup Languages
			string[] tempLanguages = lines[0].Split(';'); 
			Languages = new string[tempLanguages.Length - 1];
			for (int i = 0; i < tempLanguages.Length - 1; i++)
				Languages[i] = tempLanguages[i + 1];

			CurrentLanguage = Languages[0];

			int langCount = Languages.Length;

			for (int i = 1; i < stringCount; i++)
			{
				//Split the line to individual translated strings
				IgnoreEscapeChar(out List<string> stringTranslations, lines[i].Split(';').ToList());

				//Check if there is a corresponding enum value
				if (!Enum.TryParse(stringTranslations[0], out Translation id))
				{
					Debug.LogError(
						$"Translation enum does not contain a value of '{stringTranslations[0]}'! Did you forgot to update the Translations enum?");
					continue;
				}

				TranslationString temp = new TranslationString(langCount);

				for (int j = 0; j < langCount; j++)
				{
					temp.translationDict.Add(Languages[j],
						stringTranslations.Count > j + 1
							? stringTranslations[j + 1].Trim('"')
							: stringTranslations[0]);
				}

				translations.Add(id, temp);
			}
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

		/// <summary>
		/// Returns a localized string current language
		/// </summary>
		/// <param name="id">String id</param>
		/// <param name="parameters">Parameters for the string</param>
		/// <returns></returns>
		public static string Get(Translation id, params object[] parameters)
		{
			return Get(CurrentLanguage, id, parameters);
		}

		/// <summary>
		/// Returns localized string a requested language
		/// </summary>
		/// <param name="language">Requested language</param>
		/// <param name="id">Requested string</param>
		/// <param name="parameters">Parameters for the string</param>
		public static string Get(string language, Translation id, params object[] parameters)
		{
			if (Translations.ContainsKey(id))
				return string.Format(Translations[id].Get(language), parameters);

			Debug.LogErrorFormat("Translations does not contain a string with id '<color=red>{0}</color>'!", id);
			return $"{id} ({language})";
		}
	}

	public readonly struct TranslationString
	{
		/// <summary>
		/// A Language - Translation dictionary
		/// </summary>
		public readonly Dictionary<string, string> translationDict; 

		public TranslationString(int languageCount)
		{
			translationDict = new Dictionary<string, string>(languageCount);
		}

		/// <summary>
		/// Returns the translation of the string a given language
		/// </summary>
		/// <param name="language">Language to translate the string to</param>
		public string Get(string language)
		{
			//Check if we have a translation for the requested language
			if (translationDict.ContainsKey(language)) 
				return translationDict[language];

			Debug.LogError($"Translation does not contain language '<color=red>{language}</color>'!");
			return $"N/A language '{language}'";
		}
	}
}