using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerShop : MonoBehaviour
{
    //プレイヤの入力
    private PlayerInput input;

    //設置出来る距離
    private const float rayDistance = 5.0f;

    //タレット設置の遅延時間
    private float endTime = 0f;
    private float delay = 3.0f;

    //自販機（？）を取得 
    private GameObject shop = null;

    private Ray ray;
    private RaycastHit hitInfo;

    private Transform rayDestination;

    [SerializeField] private LayerMask shopLayer;    

    private bool stanby;

    ShopUI UIInstnace;

    private bool isAvailable { get { return endTime + delay < Time.time; } }
    // Start is called before the first frame update
    private void Awake()
    {
        input = GetComponent<PlayerInput>();
        UIInstnace = ShopUI.Instance ?? null;
        if (UIInstnace) UIInstnace.closeUI += EnableInput;

        rayDestination = GameObject.FindWithTag("CrossHairTarget").transform;
        if (rayDestination == null) { Debug.Log("turretRayDestination is Null"); }
    }

    // Update is called once per frame
    void Update()
    {
        if (!input || !input.enabled) { return; }
        if(!UIInstnace) UIInstnace = ShopUI.Instance ?? null;
        if (shop && input.InterAction && UIInstnace && isAvailable)
        {
            input.enabled = false;

            UIInstnace.OpenUI();
            stanby = true;
            shop = null;
        }

        if (!stanby)
        {
            GetShop();
        }

        bool isActive = shop != null;
        UIInstnace?.SetGuideText(isActive);
    }

    private void EnableInput()
    {
        input.enabled = true;
        stanby = false;
        endTime = Time.time;
    }

    //レイを通してスロットを取得
    public void GetShop()
    {
        Vector3 start = transform.position + Vector3.up;
        Vector3 direction = rayDestination.position - start;

        ray.origin = start;
        ray.direction = direction;

        //Debug.DrawRay(start, direction.normalized * rayDistance, Color.red);

        if (Physics.Raycast(ray, out hitInfo, rayDistance, shopLayer))
        {
            var disToTarget = Vector3.Distance(transform.position, hitInfo.transform.position);            
            if (disToTarget < 1.8f)
            {
                
                var newShop = hitInfo.collider.gameObject;
                if (newShop != null)
                {
                    shop = newShop;
                }
            }
        }
        else
        {
            shop = null;
        }
    }
}
