using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CooldownUI : MonoBehaviour
{
    private ControlUIManager _cooldownUIManager;
    public Image Image ;


    private void Awake() 
    {
        _cooldownUIManager = GetComponentInParent<ControlUIManager>() ;
        Image = GetComponent<Image>() ;
    }
}
