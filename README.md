# Localization
A simple way to translate strings in a project

A more detailed description can be found on my blog:
https://www.anton.website/a-simple-localization-system/
and
https://www.anton.website/upgrading-the-localization-system/.
These posts are describing earlier versions of this system.

# Initial setup
1. Open Localization window from the Localization -> Show Localization menu.
2. Click to Load localization file button
  a) If exists, Localization.csv file will be loaded from StreamingAssets folder 
  b) If not a default localization data will be loaded
3. Add new strings with + button on the left side of a string.
4. Reset a string with R button on the left side if a string.
5. Click E to edit a string.
  a) Edit ids and translations for each language (ids have to be unique)
  b) Save or delete a string with corresponding buttons
6. Add a new language with "+ New Language" button in the right bottom corner
7. Click "Save file" button in the left bottom corner to save the csv file 
(if a Localization.csv file already exits, it will be renamed to Localization_backup.csv)
8. Click "Generate Translation.cs" button in the bottom center to generate Translation enum (default one is in Scripts/Localization)
9. Click "Close" button in the top right corner to unload the localization data

# UI Localization
1. Add StringLocalizer.cs script to a game object with TMPro.TextMeshProUGI component attached
2. Set the relevant string id
3. Add DropdownLocalizer.cs script to a game object with TMPro.TMP_Dropdown component attached
4. Set the "Label" field with the label of the dropdown object
5. Set the relevant ids for each option

# Localization from a script
1. Use `Localizer.Get(Translation id)` method to get a translation for the current language
2. Use `Localizer.Get(string language, Translation id)` method to get a translation for a specific language
3. Use `Localizer.CurrentLanguage` to get or set the current language
  a) `Localizer.languageChanged` action is fired whenever current language is changed
4. Use `Localizer.Languages` to get a list of all languages
