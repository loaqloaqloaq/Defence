using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.VersionControl;
using UnityEngine;
using UnityEngine.UI;

public class WarningController : MonoBehaviour
{
    static WarningController instance;
    public static WarningController Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<WarningController>();
            }
            return instance;
        }
    }
    public static void ShowWarning(string key, string msg, float time = 5f) {
        Instance._ShowWarning(key, msg, time);
    }


    [SerializeField] GameObject dialogue;
    [SerializeField] TextMeshProUGUI[] messages;
    Image dialogueImage;
    [SerializeField] float showTime;
    [SerializeField] float colorChangeSpeed;
    [SerializeField] float scrollSpeed;
    Color dialogueColor;
    Vector3 textStartPos;

    RectTransform dialogueRT, maskRT;    
    Vector2 dialogueSize,maskSize;

    string msg,key;

    bool show;
    float showTimer;
    // Start is called before the first frame update
    void Start()
    {
        if(!dialogue)dialogue = transform.GetChild(0).gameObject;
        if(messages.Length<=0) messages = new TextMeshProUGUI[]{ 
            dialogue.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>(),
            dialogue.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>()
        };

        dialogueRT = dialogue.GetComponent<RectTransform>();
        maskRT = dialogue.transform.GetChild(0).GetComponent<RectTransform>();

        colorChangeSpeed = 1f;
        scrollSpeed = 500f;

        dialogueImage = dialogue.GetComponent<Image>();        
        dialogueColor = Color.red;        
        dialogueImage.color = dialogueColor;
        textStartPos = messages[0].rectTransform.anchoredPosition;

        show = false;
        showTimer = 0;

        msg = "";

        foreach (var message in messages)
        {
            message.GetComponent<ScrollingText>().scrollSpeed = scrollSpeed;
            message.GetComponent<ScrollingText>().textStartPos = textStartPos;
        }

        dialogue.SetActive(false);       
    }

    // Update is called once per frame
    void Update()
    {
        if (show)
        {
            dialogueColor.g += colorChangeSpeed * Time.deltaTime;
            dialogueImage.color = dialogueColor;
            if (dialogueColor.g >= 1) colorChangeSpeed = -1f;
            else if (dialogueColor.g <= 0) colorChangeSpeed = 1f;

            showTimer -= Time.deltaTime;
            if (showTimer <= 0) show = false;

            if (dialogue.activeSelf && dialogueSize.x < 1000)
            {
                dialogueSize.x += 2000 * Time.deltaTime;
                dialogueRT.sizeDelta = dialogueSize;
                maskSize.x += 1700 * Time.deltaTime;
                maskRT.sizeDelta = maskSize;
            }
        }
        else if (dialogue.activeSelf)
        {
            if (dialogueSize.x > 0)
            {
                dialogueSize.x -= 2000 * Time.deltaTime;
                dialogueRT.sizeDelta = dialogueSize;
                maskSize.x -= 1700 * Time.deltaTime;
                maskRT.sizeDelta = maskSize;
            }
            else Hide();
        } 
        
    }

    public void _ShowWarning(string key, string msg,float time=5f) {
        if (this.key != key)
        {            
            dialogueSize = dialogueRT.sizeDelta;
            dialogueSize.x = 0;
            dialogueRT.sizeDelta = dialogueSize;

            maskSize = maskRT.sizeDelta;
            maskSize.x = 0;
            maskRT.sizeDelta = maskSize;

            this.key = key;
            Show(msg);
        }
        else {
            SetText(msg);
        }
        showTimer = time;
    }
    void Show(string msg) {
        int index = 0;
        this.msg = msg;
        
        foreach (var message in messages)
        {
            message.GetComponent<ScrollingText>().SetMessage(msg, index);
            index++;
        }
        show = true;
        dialogue.SetActive(true);
    }
    void SetText(string msg) {
        int index = 0;
        this.msg = msg;
        foreach (var message in messages)
        {
            message.GetComponent<ScrollingText>().SetMessage(msg, index, false);
            index++;
        }
    }
    void Hide() {
        dialogue.SetActive(false);
        key = "";
             
    }
    

}
