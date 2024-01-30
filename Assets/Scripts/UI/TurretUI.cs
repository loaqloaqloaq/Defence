using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class TurretUI : MonoBehaviour
{
    private static TurretUI instance;
    public static TurretUI Instance
    {
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<TurretUI>();
            }
            return instance;
        }
    }

    private TurretSlot turretSlot = null;
    private EventSystem eventSystem;

    [SerializeField] private GameObject turretUIBackGround;
    [SerializeField] public Button firstSelectedButton;
    [SerializeField] private TextMeshProUGUI guideText;
    [SerializeField] private TextMeshProUGUI errorMessage;
    [SerializeField] private TextMeshProUGUI t1cost, t2cost, t3cost;

    //Unity Action 
    public event Action openUI;
    public event Action closeUI;

    public bool isOpened { get; private set; }

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        openUI += Enable;
        closeUI += Disable;
        errorMessage.text = "";
    }
    private void Start()
    {
        transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("{0:000000}", TurretJsonLoader.T1.cost);
        transform.GetChild(0).GetChild(1).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("{0:000000}", TurretJsonLoader.T2.cost);
        transform.GetChild(0).GetChild(2).GetChild(1).GetComponent<TextMeshProUGUI>().text = String.Format("{0:000000}", TurretJsonLoader.T3.cost);
    }

    //スロットを取得してUIを表示
    public void Setslot(TurretSlot turretSlot)
    {
        this.turretSlot = turretSlot;
        openUI();
    }

    //タレット設置・変更
    public void CreateTurret(GameObject turret)
    {
        if (!turretSlot) { return; }
        bool enoughScrap = false;
        int cost = 0;
        switch (turret.name) {
            case "Turret_1":
                enoughScrap = TurretJsonLoader.T1.cost <= GameManager.Instance.scrap;
                cost = TurretJsonLoader.T1.cost;
                break;
            case "Turret_2":
                enoughScrap = TurretJsonLoader.T2.cost <= GameManager.Instance.scrap;
                cost = TurretJsonLoader.T2.cost;
                break;
            case "Turret_3":
                enoughScrap = TurretJsonLoader.T3.cost <= GameManager.Instance.scrap;
                cost = TurretJsonLoader.T3.cost;
                break;
            default:                
                break;
        }
        Debug.Log($"cost: {cost}\n scraps:{GameManager.Instance.scrap}");
        if (!enoughScrap) {
            errorMessage.text = "NOT ENOUGH SCRAP!";
            return;
        }
        GameManager.Instance.DeductScrap(cost);
        //既にタレットが設置されているか確認
        if (turretSlot.isTurretActive)
        {
            turretSlot.ChangeTurret(turret);
        }
        else
        {
            turretSlot.CreateTurret(turret);
        }

        turretSlot = null;
        CloseUI();
    }

    //タレット除去
    public void DestroyTurret()
    {
        if (!turretSlot.isTurretActive) return;

        turretSlot.DestroyTurret();
    }

    //ガイドテキスト表示・非表示
    public void SetActiveGuideText(bool isActive)
    {
        guideText.enabled = isActive;
    }

    //UI表示・非表示 > Actionに追加して使用
    private void Enable()
    {
        if (firstSelectedButton != null) { eventSystem.SetSelectedGameObject(firstSelectedButton.gameObject); }
        errorMessage.text = "";
        t1cost.text = String.Format("{0:000000}", TurretJsonLoader.T1.cost);
        t2cost.text = String.Format("{0:000000}", TurretJsonLoader.T2.cost);
        t3cost.text = String.Format("{0:000000}", TurretJsonLoader.T3.cost);
        turretUIBackGround.SetActive(true);
        UIManager.Instance.SetMouseVisible(true);
        isOpened = true;
    }
    private void Disable()
    {
        UIManager.Instance.SetMouseVisible(false);
        turretUIBackGround.SetActive(false);
        isOpened = false;
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
