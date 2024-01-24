using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextAnimation : MonoBehaviour
{
    [SerializeField] Transform resultData;
    int current;
    int dataCount;
    float timer;
    // Start is called before the first frame update
    void Start()
    {
        if (!resultData) resultData = GameObject.Find("Result_Display").transform;
       
        dataCount =resultData.childCount;
        foreach (Transform data in resultData) { 
            data.gameObject.SetActive(false);
        }

        current = 0;
        timer = 0.5f;
    }

    // Update is called once per frame
    void Update()
    {
        if (current < dataCount) {
            timer += Time.deltaTime;
            if (timer >= 0.5f) { 
                timer = 0;
                resultData.GetChild(current).gameObject.SetActive(true);
                current++;
            }
        }
    }
}
