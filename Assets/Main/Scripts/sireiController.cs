using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class sireiController : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI tmp;
    [SerializeField] TextAsset file;
    Dictionary<string, List<string>> messages=new Dictionary<string, List<string>>();
    float timer;
    // Start is called before the first frame update
    void Awake()
    {      
        var text= file.text.Split("\n");     
        textToMessages(text);        
        timer = 2.5f;
        Debug.LogWarning(messages["Start"].Count);
        ShowText("Start");
    }   

    // Update is called once per frame
    void Update()
    {
        
    }

    void textToMessages(string[] text) {
        string key="";
        foreach(string t in text)
        {            
            if(!t.Contains("/")) continue;
            var tt= t.Split("/");
            
            if (tt[0] == "key") 
            { 
                key = tt[1];               
            }
            else if (tt[0]=="msg")
            {
                if (key == "") continue;
                else if (!messages.ContainsKey(key))  messages.Add(key, new List<string>());    
                messages[key].Add(tt[1]);               
            }
        }
        
    }
    void ShowText(string key) {        
        if (!messages.ContainsKey(key)) return;
        Debug.LogWarning(key+" "+ messages[key].Count);        
        float time = 0;
        int currentIndex=0;

        time += Time.deltaTime;
        if (time > timer)
        {
            currentIndex++;
            time = 0;
        }

        if (currentIndex < messages[key].Count)
        {
            tmp.text = messages[key][currentIndex];            
        }
        
    }
}
