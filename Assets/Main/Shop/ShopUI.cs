using TMPro;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Unity.VisualScripting;

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
    [SerializeField] private TextMeshProUGUI scraps;   
   
    [SerializeField] private SerializableKeyPair<string, Sprite>[] _imageList = default;

    private Dictionary<string, Sprite> _imageListDictionary;
    private Dictionary<string, Sprite> imageList => _imageListDictionary ??= _imageList.ToDictionary(p => p.Key, p => p.Value);   

    bool loadedItem;
    List<GameObject> buttons = new List<GameObject>();

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
        if(!scraps) scraps = transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
        if(!eventSystem) eventSystem = FindObjectOfType<EventSystem>();
        scraps.text = String.Format("{0:000000}", GameManager.Instance.scrap);
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
            GameObject button = Instantiate(Button_ShopItem, Vector3.zero, Quaternion.identity).gameObject;
            buttons.Add(button);
            var rectTransform = button.GetComponent<RectTransform>();
            rectTransform.SetParent(shopListContent.transform);
            rectTransform.localPosition = new Vector3(x, y);

            Image icon = button.transform.GetChild(0).GetComponent<Image>();
            string[] squareIconKey = { "ammo", "health" };
            if (!imageList.ContainsKey(item.name)) icon.sprite = null;
            else {
                icon.sprite = imageList[item.name];
                float height = 50;
                float width = imageList[item.name].rect.width*(height / imageList[item.name].rect.height);
                if (squareIconKey.Contains(item.name)) icon.rectTransform.sizeDelta = new Vector2(width, height);
                //Debug.Log(item.name + ": " + imageList[item.name].rect.width + " , "+imageList[item.name].rect.height);     
            }
            
            button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.name;
            button.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = String.Format("{0:0000}", item.cost);
            button.GetComponent<Button>().onClick.AddListener(()=>  Buy(item));
            
            btnCnt++;
            x += 250;
            if (btnCnt % 6==0) {
                y -= 250;
                x = 25;
            }
        }

        //set button
        for (int i = 0; i < buttons.Count; i++) {
            Button b = buttons[i].GetComponent<Button>();
            Navigation navigation = b.navigation;
            navigation.mode = Navigation.Mode.Explicit;
            if (i > 0) navigation.selectOnLeft = buttons[i - 1].GetComponent<Button>();
            else navigation.selectOnLeft = buttons[i+5].GetComponent<Button>();
            if (i > 5) navigation.selectOnLeft = buttons[i - 1].GetComponent<Button>();
            else navigation.selectOnLeft = buttons[i - 5].GetComponent<Button>();
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
        else {
            errorMessage.text = "";
            //買う
            GameManager.Instance.DeductScrap(item.cost);
            scraps.text = String.Format("{0:000000}", GameManager.Instance.scrap);
        }

        //実装中
    }

    private void Enable()
    {
        if (!loadedItem) LoadItem();
        if (buttons.Count > 0)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(buttons[0].gameObject);
        }

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
