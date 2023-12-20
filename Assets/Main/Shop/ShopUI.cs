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
    [SerializeField] private GameObject player;
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private TextMeshProUGUI guideText;
    [SerializeField] private TextMeshProUGUI errorMessage;

    [SerializeField] private GameObject Button_ShopItem;
    [SerializeField] private GameObject shopListContent;
    [SerializeField] private TextMeshProUGUI scraps;   
   
    [SerializeField] private SerializableKeyPair<string, Sprite>[] _imageList = default;    
    private Dictionary<string, Sprite> imageList => _imageList.ToDictionary(p => p.Key, p => p.Value);

    [SerializeField] private SerializableKeyPair<string, GameObject>[] _prefabList = default;
    private Dictionary<string, GameObject> prefabList => _prefabList.ToDictionary(p => p.Key, p => p.Value);

    bool loadedItem;
    List<GameObject> buttons = new List<GameObject>();
    int scarp;

    //Unity Action 
    public event Action openUI;
    public event Action closeUI;

    
    // Start is called before the first frame update
    void Start()
    {
        openUI += Enable;
        closeUI += Disable;

        if (!player) player = GameObject.Find("Player");

        if (!guideText) guideText = transform.GetChild(0).GetComponent<TextMeshProUGUI>();
        if(!shopUI) shopUI = transform.GetChild(1).gameObject;
        if(!scraps) scraps = transform.GetChild(1).GetChild(3).GetComponent<TextMeshProUGUI>();
        if(!eventSystem) eventSystem = FindObjectOfType<EventSystem>();        
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
            string[] highIconKey = { "ammo", "health", "grenade" };
            if (!imageList.ContainsKey(item.name)) icon.sprite = null;
            else {
                icon.sprite = imageList[item.name];
                if (highIconKey.Contains(item.name))
                {
                    float height = 50;
                    float width = imageList[item.name].rect.width * (height / imageList[item.name].rect.height);
                    icon.rectTransform.sizeDelta = new Vector2(width, height);
                }
                else {
                    float width = 150;
                    float height = imageList[item.name].rect.height * (width / imageList[item.name].rect.width);                    
                    icon.rectTransform.sizeDelta = new Vector2(width, height);
                }
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
            IItem shopItem = prefabList[item.name].GetComponent<IItem>()??null;
            if (shopItem != null) shopItem.Use(player);
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
        scarp = GameManager.Instance.scrap;
        scraps.text = String.Format("{0:000000}", scarp);
        errorMessage.text = "";
        openUI();
    }
    public void CloseUI()
    {
        closeUI();
    }

    private void Update()
    {
        if (scarp != GameManager.Instance.scrap) {
            scarp -=(int) ((scarp - GameManager.Instance.scrap) * 0.5f * Time.deltaTime);
            scraps.text = String.Format("{0:000000}", scarp);
        }
    }
}
