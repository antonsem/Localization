using UnityEngine;

namespace Localization
{
    /// <summary>
    /// A simlple test scene for a localization project
    /// Github: https://github.com/antonsem/Localization
    /// Blog: https://www.anton.website/a-simple-localization-system/
    /// </summary>
    public class Test : MonoBehaviour
    {
        public void ChangeLanguage(string newLanguage)
        {
            Localizer.CurrentLanguage = newLanguage;
        }
    }
}