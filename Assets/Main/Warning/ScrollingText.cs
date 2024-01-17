using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrollingText : MonoBehaviour
{
    TextMeshProUGUI message;
    [HideInInspector] public float scrollSpeed;
    [HideInInspector] public Vector3 textStartPos;
    int index;
    [SerializeField] GameObject other;
    // Start is called before the first frame update
    void Start()
    {
        message = GetComponent<TextMeshProUGUI>();
        message.text = "";
        index = -1;
    }

    // Update is called once per frame
    void Update()
    {        
        //scrolling text
        Vector3 pos = message.rectTransform.anchoredPosition;
        pos.x -= scrollSpeed * Time.deltaTime;
        message.rectTransform.anchoredPosition = pos;

        if (pos.x <= message.rectTransform.sizeDelta.x * -1 * 2) {
            if (other) SetNextToOther();
            else SetStartPos();
        }         
    }

    public void SetMessage(string msg, int index,bool start=true)
    {
        message.text = msg; 
        this.index = index;
        if(start) Invoke("SetStartPos", 0.1f);
    }
    public void SetStartPos() {
        if (index == -1)
        {
            Debug.LogWarning("warning text index error");
        }        
        Vector3 pos = textStartPos;
        pos.x += (message.rectTransform.rect.width * 2 * index);        
        message.rectTransform.anchoredPosition = pos;
    }
    void SetNextToOther() {
        Vector3 pos = other.GetComponent<RectTransform>().anchoredPosition ;
        pos.x += message.rectTransform.rect.width * 2;        
        message.rectTransform.anchoredPosition = pos;
    }

}
