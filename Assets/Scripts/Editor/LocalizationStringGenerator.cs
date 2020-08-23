using System.IO;
using UnityEditor;
using UnityEngine;

namespace Editor
{
    public static class LocalizationStringGenerator
    {
        private static string savePath = Application.dataPath;
        private const string nameSpace = "Localization";

        [MenuItem("Localization/Generate Translation Constants")]
        private static void Generate()
        {
            // Get a path to the csv file with translations
            string translationFile = EditorUtility.OpenFilePanel("Get dictionary", Application.streamingAssetsPath, "csv");
            if (string.IsNullOrEmpty(translationFile)) return;

            // Get a path for the new file
            savePath = EditorUtility.OpenFolderPanel("Save to", savePath, "Translations.cs");
            if (string.IsNullOrEmpty(savePath)) return;

            // Read the csv file with translations
            string[] lines = File.ReadAllLines(translationFile);

            if (lines.Length == 0)
            {
                Debug.LogError($"File '{translationFile}' does not contain any text, or the format is wrong!");
                return;
            }
        
            // Initialize new static class we will be saving the translations into
            string newClass = $"namespace {nameSpace}\n{{\n    public enum Translation\n    {{\n";
        
            for (int i = 0; i < lines.Length; i++)
                lines[i] = lines[i].Split(';')[0];


            // Add the ID part for each translation to the new Translations class
            for (int i = 0; i < lines.Length; i++)
            {
                // Show the progress bar
                EditorUtility.DisplayProgressBar("Writing strings", "Converting the csv file to a cs class",
                    (float) i / lines.Length);

                newClass += $"        {lines[i]} = {i.ToString()},\n";
            }

            // Add the final bracket
            newClass += "    }\n}\n";

            // Close the progress bar
            EditorUtility.ClearProgressBar();
        
            // Save the class to the path you choose in the beginning, with tne name Translations.cs
            File.WriteAllText($"{savePath}/Translations.cs", newClass);

            // Force Unity to recompile the scripts
            AssetDatabase.Refresh();
        
            // Show the success message
            EditorUtility.DisplayDialog("Translations.cs generated!", $"New class Translations.cs is saved to {savePath}!",
                "AWESOME!");
        }
    }
}