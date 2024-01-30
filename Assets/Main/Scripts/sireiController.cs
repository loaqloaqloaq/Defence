using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class sireiController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    [SerializeField] TextAsset file;
    Dictionary<string, List<string>> messages=new Dictionary<string, List<string>>();
    List<string> message;
    float timer,time;
    int currentIndex;
    static sireiController instance;
    public static sireiController Instance
    {
        get { 
            if (instance == null) instance = FindObjectOfType<sireiController>();
            return instance;
        }
    }
    public static void showText(string key) { 
        Instance.ShowText(key);
    }

    // Start is called before the first frame update
    void Awake()
    {      
        var text= file.text.Split("\n");     
        textToMessages(text);

        timer = 3.5f;

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
        //Start = 0, End1 = 1, End2 = 2, End3 = 3;
        if (key == "Start")
        {
            message = messages.ElementAt(0).Value;
        }
        else if (key == "End1") {
            message = messages.ElementAt(1).Value;
        }
        else if (key == "End2")
        {
            message = messages.ElementAt(2).Value;
        }
        else if (key == "End3")
        {
            message = messages.ElementAt(3).Value;
        }
        else if (key == "End4")
        {
            message = messages.ElementAt(4).Value;
        }
        if (message.Count <= 0)
        {
            Debug.LogWarning("message not found");
        }
    }
    void ShowText(string key) {        
        if(!tmp.gameObject.activeSelf) tmp.gameObject.SetActive(true);
        tmp.text = "";
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
        //Debug.Log(Mathf.round(time));
        if (time > timer)
        {
            currentIndex++;
            time = 0;
        }

        if (currentIndex < message.Count)
        {
            Debug.Log(message[currentIndex]);
            tmp.text = message[currentIndex];
        }
        else
        {
            time = 0;
            tmp.text = "";
            message.Clear();
        }
    }
}

