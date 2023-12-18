using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    private static ShopUI instance;  
    
    public static ShopUI Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<ShopUI>();
            }
            return instance;
        }
    }

    private EventSystem eventSystem;
    [SerializeField] private GameObject shopUI;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private TextMeshProUGUI guideText;
    [SerializeField] private TextMeshProUGUI errorMessage;

    [SerializeField] private GameObject Button_ShopItem;
    [SerializeField] private GameObject shopListContent;

    bool loadedItem;

    //Unity Action 
    public event Action openUI;
    public event Action closeUI;
    // Start is called before the first frame update
    void Start()
    {
        openUI += Enable;
        closeUI += Disable;

        if(!guideText) guideText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(!shopUI) shopUI = transform.GetChild(1).gameObject;
        if (errorMessage) errorMessage.text = "";
        guideText.enabled = false;
        shopUI.SetActive(false);

        loadedItem = false;
    }   
    
    public void SetGudieText(bool en) {
        guideText.enabled = en;
    }

    private void LoadItem() {
        float x = 25f, y = -50f;
        int btnCnt = 0;
        foreach (ShopItem item in ShopJsonLoader.Items) {            
            GameObject button = Instantiate(Button_ShopItem, Vector3.zero, Quaternion.identity);
            var rectTransform = button.GetComponent<RectTransform>();
            rectTransform.SetParent(shopListContent.transform);
            rectTransform.localPosition = new Vector3(x, y);
            button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = item.name;
            button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("{0:0000}", item.cost);
            button.GetComponent<Button>().onClick.AddListener(()=>  Buy(item));
            
            btnCnt++;
            x += 250;
            if (btnCnt >= 6) {
                y -= 250;
                x = 25;
            }
        }
        loadedItem = true;
    }
    private void Buy(ShopItem item) {
        Debug.Log("Buy:"+item.name);
        //クスラップを確認
        if (GameManager.Instance.scrap < item.cost)
        {
            errorMessage.text = "NOT ENOUGH SCRAP!";
            return;
        }
        //買う
        //実装中
    }

    private void Enable()
    {
        if (!loadedItem) LoadItem();
        if (firstSelectedButton != null) { eventSystem.SetSelectedGameObject(firstSelectedButton.gameObject); }

        shopUI.SetActive(true);
        UIManager.Instance.SetMouseVisible(true);
        playerUI?.SetActive(false);
    }

    private void Disable()
    {
        UIManager.Instance.SetMouseVisible(false);
        shopUI.SetActive(false);
        playerUI?.SetActive(true);
    }

    //UNIY Action
    public void OpenUI()
    {
        openUI();
    }
    public void CloseUI()
    {
        closeUI();
    }
}
