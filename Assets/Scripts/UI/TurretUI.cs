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

    //Unity Action 
    public event Action openUI;
    public event Action closeUI;

    public bool isOpened { get; private set; }

    private void Awake()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        openUI += Enable;
        closeUI += Disable;
    }

    //�X���b�g���擾����UI��\��
    public void Setslot(TurretSlot turretSlot)
    {
        this.turretSlot = turretSlot;
        openUI();
    }

    //�^���b�g�ݒu�E�ύX
    public void CreateTurret(GameObject turret)
    {
        if (!turretSlot) { return; }

        //���Ƀ^���b�g���ݒu����Ă��邩�m�F
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

    //�^���b�g����
    public void DestroyTurret()
    {
        if (!turretSlot.isTurretActive) return;

        turretSlot.DestroyTurret();
    }

    //�K�C�h�e�L�X�g�\���E��\��
    public void SetActiveGuideText(bool isActive)
    {
        guideText.enabled = isActive;
    }

    //UI�\���E��\�� > Action�ɒǉ����Ďg�p
    private void Enable()
    {
        if (firstSelectedButton != null) { eventSystem.SetSelectedGameObject(firstSelectedButton.gameObject); }

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
