using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScrapUI : MonoBehaviour
{
    TextMeshProUGUI text;
    // Start is called before the first frame update
    void Start()
    {
        text=GetComponent<TextMeshProUGUI>();
    }
   
    public void SetScrapText() {
        text.text = String.Format("{0:000000}", GameManager.Instance.scrap);
    }
}
