using TMPro;
using UnityEngine;
using System;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

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

    [SerializeField] private GameObject npc;

    [SerializeField] private SerializableKeyPair<string, Sprite>[] _imageList = default;    
    private Dictionary<string, Sprite> imageList => _imageList.ToDictionary(p => p.Key, p => p.Value);

    [SerializeField] private SerializableKeyPair<string, GameObject>[] _prefabList = default;
    private Dictionary<string, GameObject> prefabList => _prefabList.ToDictionary(p => p.Key, p => p.Value);

    bool loadedItem;

    public bool isOpened;
    Dictionary<string, GameObject> buttons = new Dictionary<string, GameObject>();

    [SerializeField]GameObject closeBtn;
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

        isOpened = false;

        loadedItem = false;
    }   
    
    public void SetGuideText(bool en) {
        guideText.enabled = en;
    }

    private void LoadItem() {
        float x = 25f, y = -50f;
        int btnCnt = 0;
        
        foreach (ShopItem item in ShopJsonLoader.Items) {  
            GameObject button = Instantiate(Button_ShopItem, Vector3.zero, Quaternion.identity).gameObject;
            buttons.Add(item.name,button);
            var rectTransform = button.GetComponent<RectTransform>();
            rectTransform.SetParent(shopListContent.transform);
            rectTransform.localPosition = new Vector3(x, y);

            Image icon = button.transform.GetChild(0).GetComponent<Image>();
            
            if (!imageList.ContainsKey(item.name)) icon.sprite = null;
            else {
                icon.sprite = imageList[item.name];
                float height = 50;
                float width = imageList[item.name].rect.width * (height / imageList[item.name].rect.height);
                if (width >= 150) {
                    width = 150;
                    height = imageList[item.name].rect.height * (width / imageList[item.name].rect.width);
                }
                icon.rectTransform.sizeDelta = new Vector2(width, height);                
            }
            
            button.transform.GetChild(1).GetComponent<TextMeshProUGUI>().text = item.name;
            button.transform.GetChild(2).GetComponent<TextMeshProUGUI>().text = String.Format("{0:00000}", item.cost);
            button.GetComponent<Button>().onClick.AddListener(()=>  Buy(item));
            
            btnCnt++;
            x += 250;
            if (btnCnt % 6==0) {
                y -= 250;
                x = 25;
            }
        }
        UpdateButtons();
        loadedItem = true;
    }
    private void Buy(ShopItem item) {
        Debug.Log("Buy:"+item.name);
        //クスラップを確認
        if (Scrap < item.cost)
        {
            ShowMessage("NOT ENOUGH SCRAP!");           
            return;
        }
        else {
            bool success = true;
            errorMessage.text = "";
            //買う
            Scrap -= item.cost;           
            if (prefabList.ContainsKey(item.name)) prefabList[item.name].GetComponent<IItem>().Use(player,true);
            else {
                switch (item.name) {
                    case "allies":
                        if (!BuyAllies())
                        {
                            Scrap += item.cost;
                            success = false;
                        }
                        break;
                    default:
                        ShowMessage("ITEM NOT FOUND!");
                        Scrap += item.cost;
                        success = false;
                        break;
                }
            } 
            if (success)
            {                
                ShowMessage("SUCCESS!", Color.green);
                UpdateButtons();                
            }
        }        
    }
    private bool BuyAllies() {
        if (GameManager.Instance.isNPCFull()) {
            ShowMessage("CANNOT BUY MORE ALLIES");
            return false;
        }
        GameObject allies = Instantiate(npc, getAlliesFirstCheckPoint(), Quaternion.identity);

        if (allies)
        {
            var pool = GameObject.Find("NPCPool");
            if (!pool) pool = new GameObject("NPCPool");
            allies.transform.parent = pool.transform;
        }
        
        return true;

    }
    private Vector3 getAlliesFirstCheckPoint(int area = -1)
    {
        var routes = GameObject.Find("NPCRoutes").transform;
        if (area == -1) area = GameManager.Instance.currentStage;
        if (area >= routes.childCount)
        {
            Debug.Log($"route {area} not exist");//ルート見つからない時LOGに書く
            return Vector3.zero; //テレポート中止
        }               
        var route = routes.GetChild(area);
        var checkpoints = Array.FindAll(route.GetComponentsInChildren<Transform>(), child => child != route.transform);        
        var checkpoint = checkpoints[UnityEngine.Random.Range(0, checkpoints.Length)];
        var rand = new Vector3(UnityEngine.Random.Range(-2, 2), 0, UnityEngine.Random.Range(-2, 2));
        var pos = checkpoint.GetComponent<Checkpoint>().GetPos(rand);        
        return pos;
    }
    private void Enable()
    {
        if (!loadedItem) LoadItem();
        if (buttons.Count > 0)
        {
            eventSystem.SetSelectedGameObject(null);
            eventSystem.SetSelectedGameObject(Buttons(0));
            UpdateButtons();
        }
        scrap=GameManager.Instance.scrap;
        scraps.text = String.Format("{0:000000}", GameManager.Instance.scrap);
        shopUI.SetActive(true);
        UIManager.Instance.SetMouseVisible(true);
        playerUI?.SetActive(false);
    }
    GameObject Buttons(int index) { 
        return buttons.ElementAt(index).Value.gameObject;
    }
    int ButtonsIndex(GameObject btn) {
        return buttons.Values.ToList().IndexOf(btn);
    }
    private void Disable()
    {        
        UIManager.Instance.SetMouseVisible(false);
        shopUI.SetActive(false);
        playerUI?.SetActive(true);
        GameManager.Instance.SetScrap(Scrap);
    }

    //UNIY Action
    public void OpenUI()
    {        
        scraps.text = String.Format("{0:000000}", GameManager.Instance.scrap);
        errorMessage.text = "";
        isOpened = true;
        openUI();
    }
    public void CloseUI()
    {
        isOpened = false;
        closeUI();
    }
    

    private void UpdateButtons(string key=null) {
        var current = eventSystem.currentSelectedGameObject?.GetComponent<Button>();
        var currentIndex = current? ButtonsIndex(current.gameObject):-1;
        if (key != null)
        {
            if (buttons.ContainsKey(key))
                UpdateButtonDesc(buttons[key], key);
            else {
                ShowMessage("ERROR");               
                Debug.LogError($"button {key} not found");
            }
        }
        else {
            foreach (var (btnKey,btn) in buttons) {
                UpdateButtonDesc(btn,btnKey);                
            }
        }        
        if (current?.interactable==false) {
            FindInteratableButton(currentIndex);
        }
    }
    private void UpdateButtonDesc(GameObject btn,string key) {
        var tmp = btn.transform.Find("Desc").GetComponent<TextMeshProUGUI>();
        var b = btn.GetComponent<Button>();
        var interactable = true;
        switch (key) {
            case "allies":
                tmp.text = $"LEFT\n<b>{GameManager.Instance.MaxNPCCount-GameManager.Instance.NPCCount}/{GameManager.Instance.MaxNPCCount}</b>";
                if (GameManager.Instance.NPCCount >= GameManager.Instance.MaxNPCCount) interactable = false;
                else {
                    interactable = true;
                }
                break;
            default:
                tmp.text = "LEFT\n∞";
                break;
        }
        var item = Array.Find(ShopJsonLoader.Items, i => i.name == key);
        if (GameManager.Instance.scrap < item.cost) b.interactable = interactable && false;
        else b.interactable = interactable && true;
    }
    private void FindInteratableButton(int index) {       
        for (int i = index - 1; i > 0; i--) {
            if (Buttons(i).GetComponent<Button>().interactable)
            {                
                eventSystem.SetSelectedGameObject(Buttons(i));
                return;
            }
        }
        for (int i = index + 1; i < buttons.Count; i++) {
            if (Buttons(i).GetComponent<Button>().interactable)
            {
                eventSystem.SetSelectedGameObject(Buttons(i));
                return;
            }
        }        
        if (closeBtn) eventSystem.SetSelectedGameObject(closeBtn);
    }
    //数字アニメション用
    private Coroutine startedCoroutine;
    public int CountFPS = 60;
    public float Duration = 0.5f;

    private void UpdateScrap(int target) {
        if (startedCoroutine != null) StopCoroutine(startedCoroutine);
        startedCoroutine = StartCoroutine(NumberAnimation(target));
    }
    private IEnumerator NumberAnimation(int target) {
        WaitForSeconds wait = new WaitForSeconds(1f/CountFPS);
        int prev = scrap;
        int stepAmount;
        if (target - prev < 0) {
            stepAmount = Mathf.FloorToInt( (target - prev) / (CountFPS * Duration));
        }
        else {
            stepAmount = Mathf.CeilToInt((target - prev) / (CountFPS * Duration));
        }
        if (prev < target)
        {
            while (prev < target)
            {
                prev += stepAmount;
                if (prev > target) prev = target;                
                scraps.text = String.Format("{0:000000}", prev);
                yield return wait;
            }            
        }
        else {
            while (prev > target)
            {
                prev += stepAmount;
                if (prev < target) prev = target;              
                scraps.text = String.Format("{0:000000}", prev);
                yield return wait;
            }            
        }        
    }
    void ShowMessage(string msg, Color? color = null)
    {
        if (color == null) color = Color.red;
        if (msg != null)
        {
            errorMessage.color = (Color)color;
            errorMessage.text = msg;
        }
    }
}
