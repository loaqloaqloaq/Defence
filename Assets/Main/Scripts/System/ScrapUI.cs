using System;
using System.Collections;
using TMPro;
using UnityEngine;

public class ScrapUI : MonoBehaviour
{
    TextMeshProUGUI text;
    int scrap;
    int Scrap
    {
        get {
            return scrap;
        }
        set {
            UpdateScrap(value);
            scrap = value;
        }
    }
    // Start is called before the first frame update   
    private void Awake()
    {
        text = GetComponent<TextMeshProUGUI>();
        scrap = GameManager.Instance.scrap;
        text.text = String.Format("{0:000000}", scrap);
    }

    public void SetScrapText() {
        if(gameObject.activeSelf) Scrap = GameManager.Instance.scrap;       
    }

    private Coroutine startedCoroutine;
    public int CountFPS = 60;
    public float Duration = 0.5f;
    private void UpdateScrap(int target)
    {
        if (startedCoroutine != null) StopCoroutine(startedCoroutine);
        startedCoroutine = StartCoroutine(NumberAnimation(target));
    }   
    private IEnumerator NumberAnimation(int target)
    {        
        WaitForSeconds wait = new WaitForSeconds(1f / CountFPS);
        int prev = scrap;
        int stepAmount;
        if (target - prev < 0)
        {
            stepAmount = Mathf.FloorToInt((target - prev) / (CountFPS * Duration));
        }
        else
        {
            stepAmount = Mathf.CeilToInt((target - prev) / (CountFPS * Duration));
        }
        if (prev < target)
        {
            while (prev < target)
            {
                prev += stepAmount;
                if (prev > target) prev = target;                
                text.text = String.Format("{0:000000}", prev);
                yield return wait;
            }
        }
        else
        {
            while (prev > target)
            {
                prev += stepAmount;
                if (prev < target) prev = target;                
                text.text = String.Format("{0:000000}", prev);
                yield return wait;
            }
        }
    }
}
