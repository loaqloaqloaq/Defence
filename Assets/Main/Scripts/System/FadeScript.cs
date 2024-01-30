using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FadeScript : MonoBehaviour
{
    RawImage image;
    Color c;   
    float duration;
    [SerializeField] bool fadein;
    // Start is called before the first frame update
    void Start()
    {
        image=GetComponent<RawImage>();

        if (!fadein)  c.a = 0;            
        else c.a = 1;

        image.color = c;
        duration = 2;
    }

    // Update is called once per frame
    void Update()
    {
        float amount = (1 / duration) * Time.deltaTime;
        if (!fadein)
        {
            c.a = Mathf.Min(1, c.a + amount);            
        }
        else
        {
            c.a = Mathf.Max(0, c.a - amount);
            if (c.a <= 0) gameObject.SetActive(false);
        }
        image.color = c;       
    }
}
