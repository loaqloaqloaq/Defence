using Cinemachine;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAiming : MonoBehaviour
{
    //カメラ回転
    [SerializeField] private float turnSpeed = 15;
    [SerializeField] private Transform cameraLookAt;
    [Range(0.2f, 1.0f)][SerializeField] private float axisModifier = 0.8f;
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;

    private float xStartSpeed;
    private float yStartSpeed;

    public bool isAiming { get; private set; }
    public bool isSniping { get; private set; }

    //コンポネント
    private Camera mainCamera;
    private ActiveWeapon activeWeapon;

    //ズームするときのカメラアニメーション
    [SerializeField] private Animator camAnimator;
    [SerializeField] private CinemachineFreeLook snipingCam;
    private int isAimingParam = Animator.StringToHash("IsAiming");

    //アニメーションリギング
    [SerializeField] Rig aimLayer;

    private const float zoomRecoil = 0.3f;

    //入力
    PlayerInput playerInput;

    //コンポネント取得
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        mainCamera = Camera.main;
        activeWeapon = GetComponent<ActiveWeapon>();
    }

    private void Start()
    {
        xStartSpeed = xAxis.m_MaxSpeed;
        yStartSpeed = yAxis.m_MaxSpeed;
    }

    void FixedUpdate()
    {
        SetCameraRotationSpeed();
        CameraRotate();
    }

    //マウスによるカメラ回転
    private void CameraRotate()
    {
        xAxis.Update(Time.fixedDeltaTime);
        yAxis.Update(Time.fixedDeltaTime);

        cameraLookAt.eulerAngles = new Vector3(yAxis.Value, xAxis.Value, 0);

        float yawCamera = mainCamera.transform.rotation.eulerAngles.y;
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, yawCamera, 0), turnSpeed * Time.fixedDeltaTime);
    }

    private void SetCameraRotationSpeed()
    {
        if (isSniping)
        {
            xAxis.m_MaxSpeed = xStartSpeed * axisModifier * 0.4f;
            yAxis.m_MaxSpeed = yStartSpeed * axisModifier * 0.4f;
        }
        else if (isAiming)
        {
            xAxis.m_MaxSpeed = xStartSpeed * axisModifier;
            yAxis.m_MaxSpeed = yStartSpeed * axisModifier;
        }
        else
        {
            xAxis.m_MaxSpeed = xStartSpeed;
            yAxis.m_MaxSpeed = yStartSpeed;
        }
    }

    private void Update()
    {
        if (aimLayer)
        {
            aimLayer.weight = 1.0f;
        } //AimLayerを取得しているか確認

        if (isSniping)
        {

            return;
        }

        isAiming = playerInput.zoom || playerInput.Zoom >= 0.02f;
        //カメラズームアニメーション
        camAnimator.SetBool(isAimingParam, isAiming);
        //クロスヘアイメージサイズ変更
        UIManager.Instance.SetCrosshairSize(isAiming);

        SetActiveWeaponRecoil();
    }

    //反動の設定
    private void SetActiveWeaponRecoil()
    {
        var weapon = activeWeapon.GetActiveWeapon();
        if (weapon)
        {
            if (isAiming)
            {
                weapon.recoil.recoilModifier = zoomRecoil;
            }
            else
            {
                weapon.recoil.recoilModifier = 1.0f;
            }
        }
    }

    public void OnSniping()
    {
        isSniping = true;
        snipingCam.Priority = 15;

        var uiManager = UIManager.Instance;

        if (uiManager == null)
        {
            return;
        }

        uiManager.SetEnableCanvas_Player(false);
        uiManager.SetEnableCanvas_Sniping(true);
    }

    public void OffSniping()
    {
        isSniping = false;
        snipingCam.Priority = 5;

        var uiManager = UIManager.Instance;

        if (uiManager == null)
        {
            return;
        }

        uiManager.SetEnableCanvas_Player(true);
        uiManager.SetEnableCanvas_Sniping(false);
    }

}
