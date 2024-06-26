using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Helpers;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StateDisplayController : MonoBehaviour
{
    
    public Sprite[] sprites;
    public Button playJumpAndRun;
    public Image imageState;
    public TMP_Text textState;

    private string r;
    private string k;
    
    IEnumerator Start()
    {
        yield return new WaitUntil(() => GameState.Instance.currentGameState != null);
        
        LocalizationSettings.SelectedLocaleChanged += LocalizationChanged;
        
        switch (Math.Min(GameState.Instance.currentGameState.results.Length, 5))
        {
            case 1:
            case 2:
                imageState.sprite = sprites[0];
                break;
            case 3:
            case 4:
                imageState.sprite = sprites[1];
                break;
            case 5:
                imageState.sprite = sprites[2];
                break;
        }
        imageState.SetNativeSize();
        switch (Math.Min(GameState.Instance.currentGameState.results.Length, 5))
        {
            case 1: 
            case 2:
            case 3:
                r = ((int)GameState.Instance.currentGameState.results.Last().game + 1).ToString();
                k = ((int)GameState.Instance.currentGameState.results.Last().game + 1) > 1 ? "main_menu_default_n": "main_menu_default_1";
                break;
            case 4:
                r = "";
                k = "main_menu_4";
                break;
            case 5:
                r = GameState.Instance.currentGameState.name;
                k = "main_menu_all";
                break;
            default:
                Debug.Log("is default");
                k = "main_menu_default_1";
                r = "";
                break;
        }
        Utility.GetTranslatedText(k, s =>
        {
            try
            {
                textState.text = s.Replace("~n", r);
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>()); 
            } catch {}
        });
        playJumpAndRun.onClick.AddListener(PlayJumpAndRun);
    }

    private void PlayJumpAndRun()
    {
        SceneManager.LoadScene("JumpNRun");
    }
    
    private void LocalizationChanged(Locale locale)
    {
        if (r == null || k == null)
        {
            return;
        }
        
        Utility.GetTranslatedText(k, s =>
        {
            try
            {
                textState.text = s.Replace("~n", r);
                LayoutRebuilder.ForceRebuildLayoutImmediate(transform.GetComponent<RectTransform>()); 
            } catch {}
        });
    }
}