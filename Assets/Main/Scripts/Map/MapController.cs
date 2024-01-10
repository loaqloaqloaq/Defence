using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapController : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    [SerializeField] public bool enable;
    [SerializeField] GameObject[] children;
    Camera cam;
    [SerializeField] float maxZoom, minZoom;
    [SerializeField] LayerMask whatIsTarget;

    RawImage miniMapImage;

    Vector2 prevPos, curPos;

    PlayerInput playerInput;

    [SerializeField]
    float[] maxZ, maxX,minZ,minX; 
    float maxZdif, maxXdif, minZdif, minXdif;
    public float zoomPresent;

    float mouseHoldTime;

    Transform player;
    void Start()
    {
        enable = false;        
        foreach (GameObject child in children)
        {
            child.SetActive(enable);
        }
        cam = children[0].GetComponent<Camera>();

        player = GameObject.FindWithTag("Player").transform;
        playerInput = player?.GetComponent<PlayerInput>() ?? null;        

        prevPos =Vector2.zero; curPos=Vector2.zero;

        maxZdif = maxZ[0] - maxZ[1];
        minZdif = minZ[0] - minZ[1];
        maxXdif = maxX[0] - maxX[1];
        minXdif = minX[0] - minX[1];

        zoomPresent = 1 - ((cam.orthographicSize - minZoom) / (maxZoom - minZoom));

        mouseHoldTime = 0;

        miniMapImage = transform.GetChild(1).GetChild(0).GetChild(0).GetComponent<RawImage>();

    }

    // Update is called once per frame
    void Update()
    {
        bool isPause = false;
        if (UIManager.Instance) isPause = UIManager.Instance.isPause;

        //マップの表示や非表示
        if (Input.GetKeyDown(KeyCode.M) && !isPause) {
            SetVisible(!enable);
        }
        //マップのコントロール
        if (enable)
        {
            //マップスケール
            if (Input.mouseScrollDelta.y != 0)
            {
                cam.orthographicSize -= Input.mouseScrollDelta.y * 5;
                if (cam.orthographicSize > maxZoom) cam.orthographicSize = maxZoom;
                else if (cam.orthographicSize < minZoom) cam.orthographicSize = minZoom;
                cam.transform.position = CheckMargin(cam.transform.position);
            }

            //マップ移動
            if (Input.GetMouseButtonDown(0))
            {
                curPos = Input.mousePosition;
                prevPos = Input.mousePosition;
                mouseHoldTime = 0;
            }
            if (Input.GetMouseButton(0))
            {
                mouseHoldTime += Time.deltaTime;
                curPos = Input.mousePosition;
                if (Vector2.Distance(prevPos, curPos) != 0)
                {
                    Vector2 dis = curPos - prevPos;
                    Vector3 pos = cam.transform.position;
                    pos.x += dis.y * (cam.orthographicSize / 200);
                    pos.z -= dis.x * (cam.orthographicSize / 200);
                    cam.transform.position = CheckMargin(pos);
                    prevPos = curPos;
                }
            }
            if (Input.GetMouseButtonUp(0)&& mouseHoldTime < 0.2f)
            {                
                
            }
        }
    }

    Vector3 CheckMargin(Vector3 pos) {
        zoomPresent = 1-((cam.orthographicSize - minZoom) / (maxZoom - minZoom));        
        float minX = this.minX[1] + minXdif * zoomPresent;
        float maxX = this.maxX[1] + maxXdif * zoomPresent;
        float minZ = this.minZ[1] + minZdif * zoomPresent;

        float maxZ = this.maxZ[1] + maxZdif * zoomPresent;


        pos.x = pos.x > maxX ? maxX : pos.x < minX ? minX : pos.x;
        pos.z = pos.z > maxZ ? maxZ : pos.z < minZ ? minZ : pos.z;
        
        return pos;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        
        if (mouseHoldTime >= 0.2f) return;
        Vector2 curosr = new Vector2(0, 0);

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapImage.rectTransform,
            eventData.pressPosition, eventData.pressEventCamera, out curosr))
        {

            Texture texture = miniMapImage.texture;
            Rect rect = miniMapImage.rectTransform.rect;

            float coordX = Mathf.Clamp(0, (((curosr.x - rect.x) * texture.width) / rect.width), texture.width);
            float coordY = Mathf.Clamp(0, (((curosr.y - rect.y) * texture.height) / rect.height), texture.height);

            float calX = coordX / texture.width;
            float calY = coordY / texture.height;


            curosr = new Vector2(calX, calY);
           
            CastRayToWorld(curosr);
        }

    }

    private void CastRayToWorld(Vector2 vec)
    {
        Ray MapRay = cam.ScreenPointToRay(new Vector2(vec.x * cam.pixelWidth,
            vec.y * cam.pixelHeight));

        RaycastHit hit;        

        if (Physics.Raycast(MapRay, out hit, Mathf.Infinity, whatIsTarget))
        {
            Debug.Log("miniMapHit: " + hit.collider.gameObject.name);
            ShopController hitObj = hit.collider.gameObject.GetComponent<ShopController>();
            if (hitObj != null) {
                player.GetComponent<PlayerTeleport>().TeleportTo(hitObj.GetSpawnPoint());
                SetVisible(false);
            }            
        }

    }

    private void SetVisible(bool en) {
        enable = en;
        foreach (GameObject child in children)
        {
            child.SetActive(enable);
        }
        if (playerInput) playerInput.enabled = !enable;
        UIManager.Instance?.SetMouseVisible(enable);
        //mapCanvas.enabled = enable;
        if (player && enable)
        {
            cam.transform.position = new Vector3(
                player.position.x,
                cam.transform.position.y,
                player.position.z
                );
            cam.transform.position = CheckMargin(cam.transform.position);
        }
    }

 }
