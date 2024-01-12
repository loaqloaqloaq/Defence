using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WarningController : MonoBehaviour
{
    static WarningController instance;
    public static WarningController Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<WarningController>();
            }
            return instance;
        }
    }
    public static void ShowWarning(string key, string msg, float time = 5f) {
        Instance._ShowWarning(key, msg, time);
    }


    [SerializeField] GameObject dialogue;
    [SerializeField] TextMeshProUGUI[] messages;
    Image dialogueImage;   
    [SerializeField] float colorChangeSpeed;
    [SerializeField] float scrollSpeed;
    Color dialogueColor;
    Vector3 textStartPos;

    RectTransform dialogueRT, maskRT;    
    Vector2 dialogueSize,maskSize;

    string key;

    [SerializeField] bool show;
    [SerializeField] float showTimer;
    // Start is called before the first frame update

    private void Awake()
    {
        dialogue = transform.GetChild(0).gameObject;
        dialogue.SetActive(true);
        if (messages.Length <= 0) messages = new TextMeshProUGUI[]{
            dialogue.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>(),
            dialogue.transform.GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>()
        };

        dialogueRT = dialogue.GetComponent<RectTransform>();
        maskRT = dialogue.transform.GetChild(0).GetComponent<RectTransform>();

        colorChangeSpeed = 1f;
        scrollSpeed = 500f;

        dialogueImage = dialogue.GetComponent<Image>();
        dialogueColor = Color.red;
        dialogueImage.color = dialogueColor;
        textStartPos = new Vector3(860, -17, 0);

        show = false;
        showTimer = 0;

        foreach (var message in messages)
        {
            message.GetComponent<ScrollingText>().scrollSpeed = scrollSpeed;
            message.GetComponent<ScrollingText>().textStartPos = textStartPos;
        }
        setTextVisible(false);
        key = "";
    }  

    // Update is called once per frame
    void Update()
    {
        if (show)
        {
            dialogueColor.g += colorChangeSpeed * Time.deltaTime;
            dialogueImage.color = dialogueColor;
            if (dialogueColor.g >= 1) colorChangeSpeed = -1f;
            else if (dialogueColor.g <= 0) colorChangeSpeed = 1f;

            if (dialogue.activeSelf && dialogueSize.x < 1000)
            {
                setTextVisible(true);
                dialogueSize.x += 2000 * Time.deltaTime;
                dialogueRT.sizeDelta = dialogueSize;
                maskSize.x += 1700 * Time.deltaTime;
                maskRT.sizeDelta = maskSize;
            }
            else
            {
                showTimer -= Time.deltaTime;
                if (showTimer <= 0) show = false;
            }
        }
        else if (dialogue.activeSelf)
        {
            if (dialogueSize.x > 0)
            {
                setTextVisible(false);
                dialogueSize.x -= 2000 * Time.deltaTime;
                dialogueRT.sizeDelta = dialogueSize;
                maskSize.x -= 1700 * Time.deltaTime;
                maskRT.sizeDelta = maskSize;
            }
            else Hide();
        }
        else {
            dialogue.SetActive(false);
        }
        
    }

    public void _ShowWarning(string key, string msg,float time=5f) {
        if (this.key != key)
        {            
            dialogueSize = dialogueRT.sizeDelta;
            dialogueSize.x = 0;
            dialogueRT.sizeDelta = dialogueSize;

            maskSize = maskRT.sizeDelta;
            maskSize.x = 0;
            maskRT.sizeDelta = maskSize;

            this.key = key;
            Show(msg);
        }
        else {
            SetText(msg);
        }
        show = true;
        showTimer = time;
    }
    void Show(string msg) {
        int index = 0;        
        dialogue.SetActive(true);

        foreach (var message in messages)
        {
            message.GetComponent<ScrollingText>().SetMessage(msg, index,true);
            index++;
        }        
        
    }
    void SetText(string msg) {
        int index = 0;       
        foreach (var message in messages)
        {
            message.GetComponent<ScrollingText>().SetMessage(msg, index, false);
            index++;
        }
    }
    void Hide() {
        dialogue.SetActive(false);
        key = "";
             
    }

    void setTextVisible(bool visible) {
        foreach (var message in messages)
        {
            message.enabled = visible;            
        }
    }
    

}
