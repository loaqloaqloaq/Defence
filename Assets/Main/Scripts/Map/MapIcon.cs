using UnityEngine;
using UnityEngine.UI;

public class MapIcon : MonoBehaviour
{
    [SerializeField] EnemyGloable eg; 
    [SerializeField] MapController mc;    
    [SerializeField]
    Canvas miniMapIcon, bigMapIcon;
    
    float currentZoom;
    bool notChara;   

    GameObject player;

    float normalScaleX, normalScaleY;

    // Start is called before the first frame update
    void Start()
    {
        eg = GameObject.Find("EnemyLoader")?.GetComponent<EnemyGloable>();
        mc = GameObject.Find("UiMap")?.GetComponent<MapController>();
        player = GameObject.FindWithTag("Player");

        if (bigMapIcon == null) bigMapIcon = transform.GetChild(0).GetComponent<Canvas>();
        if (miniMapIcon == null) miniMapIcon = transform.GetChild(1).GetComponent<Canvas>();
        if (eg)
        {
            bigMapIcon.worldCamera = eg.mapCam;
            miniMapIcon.worldCamera = eg.miniMapCam;
        }

        notChara = transform.parent.name.StartsWith("Weapon")||transform.parent.name.StartsWith("Grenade")|| transform.parent.name.StartsWith("enemyBase") || transform.parent.name.StartsWith("Gate") || transform.parent.name.StartsWith("Shop");
        

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

        if (notChara)
        {
            bigMapIcon.transform.eulerAngles = new Vector3(90, -90, 0);
            miniMapIcon.transform.eulerAngles = new Vector3(90, player.transform.eulerAngles.y, 0);
        }
        


    }

    void SetScale() {
        if (mc) currentZoom = mc.zoomPresent;
        var scaleX = normalScaleX * (1 - 0.5f * currentZoom);
        var scaleY = normalScaleY * (1 - 0.5f * currentZoom);
        bigMapIcon.transform.localScale = new Vector3(scaleX, scaleY, 1);
    }

    public void SetFill(float hp) {
        if (bigMapIcon.transform.GetChild(0).GetChild(0)) bigMapIcon.transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1 - hp;
        if (miniMapIcon.transform.GetChild(0).GetChild(0)) miniMapIcon.transform.GetChild(0).GetChild(0).GetComponent<Image>().fillAmount = 1 - hp;        
    }
}
