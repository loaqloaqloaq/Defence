using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MapController : MonoBehaviour, IPointerClickHandler
{
    // Start is called before the first frame update
    public static MapController instance;
    public static MapController Instance
    {
        get { 
            return instance ?? FindObjectOfType<MapController>();
        }
    }
    [SerializeField] public bool enable;
    [SerializeField] GameObject[] children;
    Camera cam;
    [SerializeField] float maxZoom, minZoom;
    [SerializeField] LayerMask whatIsTarget;
    [SerializeField] RectTransform cursor;

    RawImage miniMapImage;

    Vector2 prevPos, curPos;

    PlayerInput playerInput;

    [SerializeField]
    float[] maxZ, maxX,minZ,minX; 
    float maxZdif, maxXdif, minZdif, minXdif;
    public float zoomPresent;

    [SerializeField] float curMaxY, curMinY,curMaxX,curMinX;

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
        if (Input.GetKeyDown(KeyCode.M) || Input.GetKeyDown("joystick button 6") && !isPause) {
            SetVisible(!enable);
        }
        //マップのコントロール
        if (enable)
        {
            //マップスケール
            //マウス
            if (Input.mouseScrollDelta.y != 0)
            {
                cam.orthographicSize -= Input.mouseScrollDelta.y * 200 * Time.deltaTime;
                if (cam.orthographicSize > maxZoom) cam.orthographicSize = maxZoom;
                else if (cam.orthographicSize < minZoom) cam.orthographicSize = minZoom;
                cam.transform.position = CheckMargin(cam.transform.position);
            }
            //ゲームパッド
            if (Input.GetKey("joystick button 4")) {               
                cam.orthographicSize +=  200 * Time.deltaTime;
                if (cam.orthographicSize > maxZoom) cam.orthographicSize = maxZoom;
            }
            if (Input.GetKey("joystick button 5")) {
                cam.orthographicSize -=  200 * Time.deltaTime;
                if (cam.orthographicSize < minZoom) cam.orthographicSize = minZoom;
            }

            //マップ移動
            //マウス
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
            //ゲームパッド
            if (Input.GetAxis("RStick X") != 0 || Input.GetAxis("RStick Y") != 0) {
                var rx = Input.GetAxis("RStick X");
                var ry = Input.GetAxis("RStick Y");
                Vector3 rinput =new Vector3(ry, 0, rx);
                Vector3 pos = cam.transform.position;
                pos -= (rinput*Time.deltaTime*200);
                cam.transform.position = CheckMargin(pos);
            }
            
            if (GameManager.LastInputDevice == InputDevice.GAMEPAD)
            {
                if(cursor?.gameObject.activeSelf==false) cursor.gameObject.SetActive(true);
                if (cursor?.gameObject.activeSelf == true) {
                    if (Input.GetAxis("LStick X") != 0 || Input.GetAxis("LStick Y") != 0)
                    {
                        var rx = Input.GetAxis("LStick X");
                        var ry = Input.GetAxis("LStick Y");
                        Vector2 rinput = new Vector2(rx, ry);
                        Vector2 pos = cursor.anchoredPosition;
                        pos += (rinput * Time.deltaTime * 200);
                        cursor.anchoredPosition = CheckCursorMargin(pos);
                    }
                }
                if (Input.GetButtonDown("Submit")) {
                    Vector2 pos = new Vector2(0, 0);
                    if (RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapImage.rectTransform, cursor.anchoredPosition, cam, out pos))
                    {
                        Texture texture = miniMapImage.texture;
                        Rect rect = miniMapImage.rectTransform.rect;
                        float coordX = Mathf.Clamp(0, (((pos.x - rect.x) * texture.width) / rect.width), texture.width);
                        float coordY = Mathf.Clamp(0, (((pos.y - rect.y) * texture.height) / rect.height), texture.height);
                        Vector2 coord = new Vector2(coordX / texture.width, coordY / texture.height);
                        Debug.Log(coord);
                    }
                }
            }
            else {
                if (cursor?.gameObject.activeSelf == true) cursor.gameObject.SetActive(false);
            }
        }
    }
    Vector2 CheckCursorMargin(Vector2 pos) {
        pos.x = pos.x > curMaxX ? curMaxX : pos.x < curMinX ? curMinX : pos.x;
        pos.y = pos.y > curMaxY ? curMaxY : pos.y < curMinY ? curMinY : pos.y;
        return pos;
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

        if (RectTransformUtility.ScreenPointToLocalPointInRectangle(miniMapImage.rectTransform,eventData.pressPosition, eventData.pressEventCamera, out curosr))
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

    public void SetVisible(bool en) {
        enable = en;
        foreach (GameObject child in children)
        {
            child.SetActive(enable);
        }
        if (playerInput) playerInput.enabled = !enable;
        UIManager.Instance?.SetMouseVisible(enable);
        //mapCanvas.enabled = enable;
        if (enable)
        {
            if (player)
            {
                cam.transform.position = new Vector3(
                    player.position.x,
                    cam.transform.position.y,
                    player.position.z
                    );
                cam.transform.position = CheckMargin(cam.transform.position);
            }
            if (cursor) {
                cursor.anchoredPosition = Vector2.zero;
            }
        }
    }

 }
