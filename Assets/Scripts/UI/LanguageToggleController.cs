using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Helpers;
using Models;
using Nobi.UiRoundedCorners;
using TMPro;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;


public class LanguageToggleController : MonoBehaviour
{
    public Sprite de;
    public Sprite en;
    private Language _currentLanguage;
    private Image _image;
    
    void Start()
    {
        _currentLanguage = Application.systemLanguage == SystemLanguage.German ? Language.De : Language.En;
        LocalizationSettings.SelectedLocale = _currentLanguage == Language.De ? LocalizationSettings.AvailableLocales.GetLocale("de") : LocalizationSettings.AvailableLocales.GetLocale("en");
        _image = GetComponent<Image>();
        _image.sprite = _currentLanguage == Language.De ? en : de;
        GetComponent<Button>().onClick.AddListener(ToggleLanguage);
    }

    private void ToggleLanguage()
    {
        _currentLanguage = _currentLanguage == Language.De ? Language.En : Language.De;
        _image.sprite = _currentLanguage == Language.De ? en : de;
        LocalizationSettings.SelectedLocale = _currentLanguage == Language.De ? LocalizationSettings.AvailableLocales.GetLocale("de") : LocalizationSettings.AvailableLocales.GetLocale("en");
    }
    
    public Language GetCurrentLanguage()
    {
        return _currentLanguage;
    }
}