using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.VersionControl;
using UnityEngine;

public class sireiController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    [SerializeField] TextAsset file;
    Dictionary<string, List<string>> messages=new Dictionary<string, List<string>>();
    List<string> message;
    float timer,time;
    int currentIndex;
    // Start is called before the first frame update
    void Awake()
    {      
        var text= file.text.Split("\n");     
        textToMessages(text);

        timer = 2.5f;

        ShowText("Start");
    }   

    // Update is called once per frame
    void Update()
    {
        if (message.Count > 0) {
            TextUpdate();
        }
    }

    void textToMessages(string[] text) {
        string key="";
        foreach(string t in text)
        {            
            if(!t.Contains("/")) continue;
            string[] tt= t.Split("/");
            
            if (tt[0] == "key") 
            { 
                key = tt[1];               
            }
            else if (tt[0] == "msg")
            {               
                if (key == "") continue;
                else if (!messages.ContainsKey(key))  messages.Add(key, new List<string>());    
                messages[key].Add(tt[1]);               
            }
        }        
    }

    void GetMessage(string key) {        
        foreach (var list in messages) {
            if (list.Key.CompareTo(key)==1) {
                message= list.Value;
                return;
            }
        }
        Debug.LogWarning("message not found");
    }
    void ShowText(string key) {
        time = 0;
        currentIndex = 0;
        GetMessage(key);        
    }

    void TextUpdate() {
        if (message.Count <= 0)
        {
            tmp.text = "";
            return;
        }    
        time += Time.deltaTime;
        Debug.Log(time);
        if (time > timer)
        {
            currentIndex++;
            time = 0;
        }

        if (currentIndex < message.Count)
        {
            tmp.text = message[currentIndex];
        }
        else
        {
            tmp.text = "";
            message.Clear();
        }
    }
}

