using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnimationController : MonoBehaviour
{
    private Animator _animator;
    
    private GameObject _animateStartOpenGameObject;
    private GameObject _disabledOpenButtonGameObject;
    private GameObject _animateStartCloseGameObject;
    private GameObject _disabledCloseButtonGameObject;

    private GameObject openCloseAnimationGameObject;
    
    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        _animator.enabled = true;
        
        openCloseAnimationGameObject = FindHiddenOpenCloseContainer();
        openCloseAnimationGameObject.SetActive(true);
        
        _animateStartOpenGameObject = GameObject.Find("OpenActiveButton"); 
        _animateStartCloseGameObject = GameObject.Find("CloseActiveButton"); 
        _animateStartOpenGameObject.GetComponent<Button>().onClick.AddListener(triggerAnimation);
        _animateStartCloseGameObject.GetComponent<Button>().onClick.AddListener(triggerAnimation);
        
        _disabledCloseButtonGameObject = GameObject.Find("CloseDisabledButton");
        _disabledOpenButtonGameObject = GameObject.Find("OpenDisabledButton");
        _animateStartCloseGameObject.SetActive(false);
        _disabledOpenButtonGameObject.SetActive(false);
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(openCloseAnimationGameObject.GetComponent<RectTransform>());
    }

    // Update is called once per frame
    private void triggerAnimation()
    {
        AnimatorStateInfo animatorState = _animator.GetCurrentAnimatorStateInfo(0);
        if (animatorState.IsName("opening") || animatorState.IsName("open"))
        {
            _animator.ResetTrigger("open");
            _animator.SetTrigger("close");
            _animateStartCloseGameObject.SetActive(false);
            _animateStartOpenGameObject.SetActive(true);
            _disabledOpenButtonGameObject.SetActive(false);
            _disabledCloseButtonGameObject.SetActive(true);
        }
        else
        {
            _animator.ResetTrigger("close");
            _animator.SetTrigger("open");
            _animateStartCloseGameObject.SetActive(true);
            _animateStartOpenGameObject.SetActive(false);
            _disabledOpenButtonGameObject.SetActive(true);
            _disabledCloseButtonGameObject.SetActive(false);
        }
        
        LayoutRebuilder.ForceRebuildLayoutImmediate(openCloseAnimationGameObject.GetComponent<RectTransform>());
    }

    private GameObject FindHiddenOpenCloseContainer()
    {
        GameObject resultGameObject = null;
        
        foreach (GameObject go in Resources.FindObjectsOfTypeAll(typeof(GameObject)) as GameObject[])
        {
            Debug.Log(go.name);
            if (go.name == "OpenCloseAnimation")
                resultGameObject = go;
        }

        return resultGameObject;
    }
}
