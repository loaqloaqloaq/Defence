using UnityEngine;

public class MapIcon : MonoBehaviour
{
    [SerializeField] EnemyGloable eg; 
    [SerializeField] MapController mc;    
    [SerializeField]
    Canvas miniMapIcon, bigMapIcon;
    
    float currentZoom;
    bool isPickUpWeapon;

    GameObject player;

    float normalScaleX, normalScaleY;

    // Start is called before the first frame update
    void Start()
    {
        eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();
        mc = GameObject.Find("UiMap").GetComponent<MapController>();
        player = GameObject.Find("Player");
        if (bigMapIcon==null) bigMapIcon = transform.GetChild(0).GetComponent<Canvas>();
        if(miniMapIcon==null) miniMapIcon = transform.GetChild(1).GetComponent<Canvas>();
        bigMapIcon.worldCamera = eg.mapCam;
        miniMapIcon.worldCamera = eg.miniMapCam;        
        isPickUpWeapon = transform.parent.name.StartsWith("Weapon")||transform.parent.name.StartsWith("Grenade");

        normalScaleX = bigMapIcon.transform.localScale.x;
        normalScaleY = bigMapIcon.transform.localScale.y;

        SetScale();

    }

    // Update is called once per frame
    void Update()
    {
        if (mc && currentZoom != mc.zoomPresent) {
            SetScale();
        }

        if (isPickUpWeapon) {
            bigMapIcon.transform.eulerAngles = new Vector3(90, -90, 0);
            miniMapIcon.transform.eulerAngles = new Vector3(90, player.transform.eulerAngles.y, 0);
        }

    }

    void SetScale() {
        currentZoom = mc.zoomPresent;
        var scaleX = normalScaleX * (1 - 0.5f * currentZoom);
        var scaleY = normalScaleY * (1 - 0.5f * currentZoom);
        bigMapIcon.transform.localScale = new Vector3(scaleX, scaleY, 1);
    }
}
