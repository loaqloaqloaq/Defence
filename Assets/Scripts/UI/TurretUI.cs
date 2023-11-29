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

    private TurretSlot slot = null;
    private EventSystem _eventSystem;

    [SerializeField] private GameObject turretUIBackGround;
    [SerializeField] private Button firstSelectedButton;
    [SerializeField] private TextMeshProUGUI guideText;

    public event Action CloseUI;

    private void Awake()
    {
        _eventSystem = FindObjectOfType<EventSystem>();
    }

    public void SetSlot(TurretSlot slot)
    {
        this.slot = slot;
        Enable();
    }

    public void Enable()
    {
        if (firstSelectedButton != null) { _eventSystem.SetSelectedGameObject(firstSelectedButton.gameObject); }

        turretUIBackGround.SetActive(true);
        UIManager.Instance.SetMouseVisible(true);
    }

    public void Disable()
    {
        UIManager.Instance.SetMouseVisible(false);
        turretUIBackGround.SetActive(false);
    }

    public void CreateTurret(GameObject turret)
    {
        if (slot)
        {
            slot.CreateTurret(turret);
        }

        Disable();
        CloseUI();
        slot = null;
    }

    public void SetActiveGuideText(bool isActive)
    {
        guideText.enabled = isActive;
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
