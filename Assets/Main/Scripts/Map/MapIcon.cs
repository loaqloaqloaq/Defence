using UnityEngine;

public class MapIcon : MonoBehaviour
{
    [SerializeField] EnemyGloable eg; 
    [SerializeField] MapController mc;    
    [SerializeField]
    Canvas miniMapIcon, bigMapIcon;
    
    float currentZoom;   

    // Start is called before the first frame update
    void Start()
    {
        eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();
        mc = GameObject.Find("UiMap").GetComponent<MapController>();
        if (bigMapIcon==null) bigMapIcon = transform.GetChild(0).GetComponent<Canvas>();
        if(miniMapIcon==null) miniMapIcon = transform.GetChild(1).GetComponent<Canvas>();
        bigMapIcon.worldCamera = eg.mapCam;
        miniMapIcon.worldCamera = eg.miniMapCam;
        currentZoom = mc.zoomPresent;
    }

    // Update is called once per frame
    void Update()
    {
        if (mc && currentZoom != mc.zoomPresent) {
            currentZoom=mc.zoomPresent;
            bigMapIcon.transform.localScale = new Vector3(1 - 0.5f * currentZoom, 1 - 0.5f * currentZoom, 1);
        }
    }
}
