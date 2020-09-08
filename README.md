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
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/2.png" alt="Read Localzation File" width="500"/> 
3. Add new strings with + button on the left side of a string.
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/3.png" alt="Read Localzation File" width="500"/> 
4. Reset a string with R button on the left side if a string.
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/4.png" alt="Read Localzation File" width="500"/> 
5. Click E to edit a string.
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/5.png" alt="Read Localzation File" width="500"/> 
  a) Edit ids and translations for each language (ids have to be unique)
  <img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/5a.png" alt="Read Localzation File" width="500"/> 
  b) Save or delete a string with corresponding buttons
  <img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/5b.png" alt="Read Localzation File" width="500"/> 
6. Add a new language with "+ New Language" button in the right bottom corner
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/6.png" alt="Read Localzation File" width="500"/> 
7. Click "X" button on the right side of a language to remove the language
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/7.png" alt="Read Localzation File" width="500"/> 
8. Click "Save file" button in the left bottom corner to save the csv file
(if a Localization.csv file already exits, it will be renamed to Localization_backup.csv)
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/8.png" alt="Read Localzation File" width="500"/>
9. Click "Generate Translation.cs" button in the bottom center to generate Translation enum (default one is in Scripts/Localization)
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/9.png" alt="Read Localzation File" width="500"/> 
10. Click "Close" button in the top right corner to unload the localization data
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/10.png" alt="Read Localzation File" width="500"/> 

# UI Localization
1. Add StringLocalizer.cs script to a game object with TMPro.TextMeshProUGI component attached
2. Set the relevant string id
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/2_2.png" alt="Read Localzation File" width="500"/> 

3. Add DropdownLocalizer.cs script to a game object with TMPro.TMP_Dropdown component attached
4. Set the "Label" field with the label of the dropdown object
5. Set the relevant ids for each option
<img src="https://github.com/antonsem/Localization/blob/master/ReadMeFiles/2_5.png" alt="Read Localzation File" width="500"/> 

# Localization from a script
1. Use `Localizer.Get(Translation id)` method to get a translation for the current language
2. Use `Localizer.Get(string language, Translation id)` method to get a translation for a specific language
3. Use `Localizer.CurrentLanguage` to get or set the current language
  a) `Localizer.languageChanged` action is fired whenever current language is changed
4. Use `Localizer.Languages` to get a list of all languages

# Escape charcter
You are free to use semicolon (;) in the custom editor UI. The asset can handle it. However, if you want to edit the Localization.csv
with an external editor add backslash (\) before the semicolon. If you need to have the backslash at the end of a string, add another
one before it.

`escapeCharacterExample_1;To add semicolon write it like this \; <-. It will work;Noktalı virgü bu şekilde eklenebilir \; <-.`

`escapeCharacterExample_2;To add the escape character to the end of a string do this: \\;İşaretini satırın sonunda şu şekilde kullanabilirsiniz: \\`

