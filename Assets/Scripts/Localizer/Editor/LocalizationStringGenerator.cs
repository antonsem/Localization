using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;

namespace Localization
{
    public static class LocalizationStringGenerator
    {
        private static string _translationsFileLocation = Application.streamingAssetsPath;
        private static string _savePath = Application.dataPath;
        private const string _nameSpace = "Localization";

        [MenuItem("Localization/Generate Translation Constants")]
        public static void Generate()
        {
            // Get a path to the csv file with translations
            _translationsFileLocation = EditorUtility.OpenFilePanel("Get dictionary", _translationsFileLocation, "csv");
            if (string.IsNullOrEmpty(_translationsFileLocation)) return;

            // Get a path for the new file
            _savePath = EditorUtility.OpenFolderPanel("Save to", _savePath, "Translations.cs");
            if (string.IsNullOrEmpty(_savePath)) return;

            // Read the csv file with translations
            string[] lines = File.ReadAllLines(_translationsFileLocation);

            if (lines.Length == 0)
            {
                Debug.LogError($"File '{_translationsFileLocation}' does not contain any text, or the format is wrong!");
                return;
            }
        
            // Initialize new static class we will be saving the translations into
            StringBuilder newClass =
                new StringBuilder($"namespace {_nameSpace}\n{{\n    public enum Translation\n    {{\n");
            
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Split(';')[0];


            // Add the ID part for each translation to the new Translations class
            for (int i = 0; i < lines.Length; i++)
            {
                // Show the progress bar
                EditorUtility.DisplayProgressBar("Writing strings", "Converting the csv file to a cs class",
                    (float) i / lines.Length);

                newClass.AppendLine($"        {lines[i]} = {i.ToString()},");
            }

            // Add the final bracket
            newClass.AppendLine("    }\n}");

            // Close the progress bar
            EditorUtility.ClearProgressBar();
        
            // Save the class to the path you choose in the beginning, with tne name Translations.cs
            File.WriteAllText($"{_savePath}/Translations.cs", newClass.ToString());

            // Force Unity to recompile the scripts
            AssetDatabase.Refresh();
        
            // Show the success message
            EditorUtility.DisplayDialog("Translations.cs generated!", $"New class Translations.cs is saved to {_savePath}!",
                "AWESOME!");
        }
    }
}