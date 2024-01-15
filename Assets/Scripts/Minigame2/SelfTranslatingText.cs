using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SelfTranslatingTest : MonoBehaviour
{

    public string translationKey;
    
    void Start()
    {
        GetComponent<TMP_Text>().text = LocalizationSettings.StringDatabase.GetLocalizedString(translationKey);
    }
}
