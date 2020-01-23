# Localization
A simple way to translate strings in a project

A more detailed description can be found on my blog: https://www.anton.website/a-simple-localization-system/

Import Localization.cs and StringLocalizer.cs. Create a CSV file inside the StreamingAssets folder.
An example CSV file can be found in the StreamingAssets folder of this repository.

To get a translated string in the code use Localizer.Get(language, ID) method. Lanugage name must
be the same as in the top row of the CSV file. The IDs can be found at the first column of the CSV
file.

To get a translated string with the currently set language use Localizer.Get(ID) method.

To localize a text component (TextmeshProUGUI) add a StringLocalizer component next to the text
component. Write the ID of the desired string as a text, and click "Get Default String" button on
the StringLocalizer component. The defaultString variable will be used as an ID for the localization.
