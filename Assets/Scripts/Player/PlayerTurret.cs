using UnityEngine;

public class PlayerTurret : MonoBehaviour
{
    //プレイヤの入力
    private PlayerInput input;

    //設置出来る距離
    private const float rayDistance = 5.0f;

    //タレット設置の遅延時間
    private float endTime = 0f;
    private float delay = 3.0f;

    //TurretSlot 取得に必要なパラメータ 
    private TurretSlot turretSlot = null;

    private Ray ray;
    private RaycastHit hitInfo;

    private Transform rayDestination;

    [SerializeField] private LayerMask turretLayer;

    private bool stanby;

    private bool isAvailable { get { return endTime + delay < Time.time; } }

    private void Awake()
    {
        input = GetComponent<PlayerInput>();

        if (TurretUI.Instance)
        {
            TurretUI.Instance.openUI += input.InitInputXY;
            TurretUI.Instance.closeUI += EnableInput;
        }

        rayDestination = GameObject.FindWithTag("CrossHairTarget").transform;
        if (rayDestination == null) { Debug.Log("turretRayDestination is Null"); }
    }

    private void Update()
    {
        if (!input || !input.enabled) { return; }

        var UIInstnace = TurretUI.Instance;
        if (turretSlot && input.InterAction && UIInstnace && isAvailable)
        {
            input.enabled = false;

            UIInstnace.Setslot(turretSlot);
            stanby = true;
            turretSlot = null;
        }

        if (!stanby)
        {
            GetSlot();
        }

        bool isActive = turretSlot != null;
        TurretUI.Instance?.SetActiveGuideText(isActive);
    }

    private void EnableInput()
    {
        input.enabled = true;
        stanby = false;
        endTime = Time.time;
    }

    //レイを通してスロットを取得
    public void GetSlot()
    {
        Vector3 start = transform.position + Vector3.up;
        Vector3 direction = rayDestination.position - start;

        ray.origin = start;
        ray.direction = direction;

        //Debug.DrawRay(start, direction.normalized * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hitInfo, rayDistance, turretLayer))
        {
            //Debug.Log(hitInfo.collider.name);
            var newSlot = hitInfo.collider.transform.GetComponent<TurretSlot>();
            if (newSlot != null) turretSlot = newSlot;
        }
        else
        {
            turretSlot = null;
        }
    }


}
