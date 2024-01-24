using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{    
    [SerializeField] List<Transform> textBox = new List<Transform>();
    int current;
    int dataCount;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        if (textBox.Count <= 0) {
            var resultData = GameObject.Find("Result_Display").transform;
            foreach (Transform data in resultData) {                
                textBox.Add(data);
            }
        }

        foreach (Transform data in textBox)
        {
            data.gameObject.SetActive(false);
        }
        dataCount = textBox.Count;        

        current = 0;
        timer = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (current < dataCount) {
            timer += Time.deltaTime;
            if (timer >= 0.5f) {
                textBox[current].gameObject.SetActive(true);
                if (textBox[current].GetComponentInChildren<TextMeshProUGUI>().text != "") timer = 0;                
                current++;
            }
        }
    }
}
