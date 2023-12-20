using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class WarningController : MonoBehaviour
{
    [SerializeField] GameObject dialogue;
    [SerializeField] TextMeshProUGUI message;
    Image dialogueImage;
    [SerializeField] float showTime;
    [SerializeField] float colorChangeSpeed;
    Color dialogueColor;
    // Start is called before the first frame update
    void Start()
    {
        if(!dialogue)dialogue = transform.GetChild(0).gameObject;
        if(!message) message = dialogue.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>();
        message.text = "";        
        colorChangeSpeed = 1f;
        dialogueImage = dialogue.GetComponent<Image>();        
        dialogueColor = Color.red;
        Debug.Log(dialogueColor.ToString());
        dialogueImage.color = dialogueColor;

        //dialogue.SetActive(false);
        showTime = 100;
    }

    // Update is called once per frame
    void Update()
    {
        if (showTime > 0) {            
            dialogueColor.g += colorChangeSpeed*Time.deltaTime;
            dialogueImage.color = dialogueColor;
            if (dialogueColor.g >= 1)  colorChangeSpeed = -1f;
            else if (dialogueColor.g <= 0) colorChangeSpeed = 1f;
        }
    }

    public void SetWarning(string msg,float time=5f) { 
    
    }
    void Show() {
        dialogue.SetActive(true);
    }
}
