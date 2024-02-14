using System.Collections;
using System.Collections.Generic;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Onboarding
{
    public class SubmitOnboarding : MonoBehaviour
    {
    
        public TMP_InputField nameInput;
        public Button submitButton;
    
        // Start is called before the first frame update
        void Start()
        {
            submitButton.onClick.AddListener(Submit);
        }

        private async void Submit()
        {
            string p = await Api.GetPlayerDetails(nameInput.text, GameObject.Find("LanguageToggle").GetComponent<LanguageToggleController>().GetCurrentLanguage());
            if (p != null)
            {
                await Api.ReserializeGamestate(p);
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}