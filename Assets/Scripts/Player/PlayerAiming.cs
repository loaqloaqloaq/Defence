using UnityEngine;
using UnityEngine.Animations.Rigging;

public class PlayerAiming : MonoBehaviour
{
    //カメラ回転
    [SerializeField] private float turnSpeed = 15;
    [SerializeField] private Transform cameraLookAt;
    public Cinemachine.AxisState xAxis;
    public Cinemachine.AxisState yAxis;
    public bool isAiming { get; private set; }

    //コンポネント
    private Camera mainCamera;
    private Animator animator;
    private ActiveWeapon activeWeapon;

    //ズームするときのカメラアニメーション
    [SerializeField] private Animator camAnimator;
    private int isAimingParam = Animator.StringToHash("IsAiming");
    
    //アニメーションリギング
    [SerializeField] Rig aimLayer;

    private const float zoomRecoil = 0.3f;

    //コンポネント取得
    void Awake()
    {
        mainCamera = Camera.main;
        animator = GetComponent<Animator>();
        activeWeapon = GetComponent<ActiveWeapon>();
    }

    void FixedUpdate()
    {
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

    private void Update()
    {
        if (aimLayer)
        {
            aimLayer.weight = 1.0f;
        } //AimLayerを取得しているか確認

        isAiming = Input.GetMouseButton(1);
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
}
