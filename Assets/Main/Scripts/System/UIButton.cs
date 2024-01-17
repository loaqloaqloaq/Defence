using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIButton : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI runText, flashLightText, mapText, grenadeText;
    [SerializeField] TextMeshProUGUI[] interactTexts;    
    [SerializeField] Transform[] weaponBtns;
    [SerializeField] Transform ButtonDesc;

    static UIButton instance;    
    public static void SetUIText(InputDevice device) {
        if (instance == null) instance = FindObjectOfType<UIButton>();
        instance?._SetUIText(device);        
    }


    public void _SetUIText(InputDevice device) {
        if (device == InputDevice.KEYBOARD) {
            if(runText) runText.text = "SHIFT:";           
            if (grenadeText) grenadeText.text = "G";

            if (interactTexts.Length>0)
            foreach(var interactText in interactTexts) {
                interactText.text = "PRESS F";
            }
            if (weaponBtns.Length > 0)
            foreach (var weaponBtn in weaponBtns) {
                weaponBtn.GetChild(0).gameObject.SetActive(true);
                weaponBtn.GetChild(1).gameObject.SetActive(false);
            }
        }
        else
        {
            if (runText) runText.text = "RT:";            
            if (grenadeText) grenadeText.text = "B";
            
            if (interactTexts.Length > 0)
            foreach (var interactText in interactTexts)
            {
                interactText.text = "PRESS Y BUTTON";
            }
            if (weaponBtns.Length > 0)
            foreach (var weaponBtn in weaponBtns)
            {
                weaponBtn.GetChild(0).gameObject.SetActive(false);
                weaponBtn.GetChild(1).gameObject.SetActive(true);
            }
        }        
        foreach (Transform desc in ButtonDesc) {            
            if(desc==ButtonDesc.GetChild((int)device)) desc.gameObject.SetActive(true);
            else desc.gameObject.SetActive(false);
        }

    }
}
