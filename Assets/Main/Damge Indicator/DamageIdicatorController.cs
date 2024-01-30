using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageIdicatorController : MonoBehaviour
{
    Image image;
    Color originalColor,c;
    float showTime;
    float showTimer;

    GameObject player,key;
    Vector3 targetPos;

    RectTransform rectTransform;

    
    // Start is called before the first frame update
    void Awake()
    {
        if(!image) image=transform.GetComponentInChildren<Image>();
        originalColor=image.color;
        player = GameObject.FindGameObjectWithTag("Player");
        rectTransform = transform.GetComponent<RectTransform>();
        targetPos = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //表示位置更新
        updateRotation();
        //表示時間過ぎた後段々薄くなる
        showTimer += Time.deltaTime;
        if (showTimer >= showTime)
        {
            if (c.a > 0)
            {
                c.a -= originalColor.a * Time.deltaTime;
                image.color = c;
            }
            else remove();

        }        
    }

    public void updatePos(GameObject key) {
        c = originalColor;
        image.color = c;
        showTime = 1;
        showTimer = 0;
        targetPos = key.transform.position;
        rectTransform.anchoredPosition = Vector2.zero;
        this.key = key;
    }

    private void updateRotation() {
        Vector3 targetDir = targetPos - player.transform.position;
        float angle = Vector3.Angle(targetDir, player.transform.forward);
        //Debug.Log(angle);
        var an=rectTransform.eulerAngles;
        an.z = angle;
        rectTransform.eulerAngles=an;
    }

    private void remove() {
        DamageIndicator.Remove(key);
    }
    
}
