using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MapController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public bool enable;
    [SerializeField] GameObject[] children;
    Camera cam;
    UIManager uiManager;

    Vector2 prevPos, curPos;

    PlayerInput playerInput;
    void Start()
    {
        enable = false;        
        foreach (GameObject child in children)
        {
            child.SetActive(enable);
        }
        cam = children[0].GetComponent<Camera>();

        playerInput=GameObject.Find("Player")?.GetComponent<PlayerInput>()??null;
        uiManager = GameObject.Find("UIManager")?.GetComponent<UIManager>() ?? null;

        prevPos=Vector2.zero; curPos=Vector2.zero;
    }

    // Update is called once per frame
    void Update()
    {
        //�}�b�v�̕\�����\��
        if (Input.GetKeyDown(KeyCode.M)) {
            enable = !enable;
            foreach (GameObject child in children) { 
                child.SetActive(enable);
            }
            if(playerInput) playerInput.enabled=!enable;
            uiManager.SetMouseVisible(enable);
            //mapCanvas.enabled = enable;
        }
        //�}�b�v�̃R���g���[��
        if (enable)
        {
            //�}�b�v�X�P�[��
            if (Input.mouseScrollDelta.y != 0)
            {
                cam.orthographicSize -= Input.mouseScrollDelta.y * 5;
                if (cam.orthographicSize > 200) cam.orthographicSize = 200;
                else if (cam.orthographicSize < 13) cam.orthographicSize = 13;
            }

            //�}�b�v�ړ�
            if (Input.GetMouseButtonDown(0))
            {
                curPos = Input.mousePosition;
                prevPos = Input.mousePosition;
            }
            if (Input.GetMouseButton(0)) {
                curPos = Input.mousePosition;
                if (Vector2.Distance(prevPos, curPos) != 0) { 
                    Vector2 dis = curPos- prevPos;
                    Vector3 pos = cam.transform.position;
                    pos.x += dis.y ;
                    pos.z -= dis.x ;
                    cam.transform.position = pos;
                    prevPos = curPos;
                }
            }
        }        
    }
}
