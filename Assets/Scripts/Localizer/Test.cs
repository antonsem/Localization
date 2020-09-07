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

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.T))
                Debug.Log(Localizer.Get(Translation.paramsTest, "1", "2"));
        }
    }
}