using UnityEngine;

public class MapController : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public bool enable;
    [SerializeField] GameObject[] children;
    Camera cam;
    [SerializeField] float maxZoom, minZoom;

    Vector2 prevPos, curPos;

    PlayerInput playerInput;

    [SerializeField]
    float[] maxZ, maxX,minZ,minX; 
    float maxZdif, maxXdif, minZdif, minXdif;
    public float zoomPresent;

    Transform player;
    void Start()
    {
        enable = false;        
        foreach (GameObject child in children)
        {
            child.SetActive(enable);
        }
        cam = children[0].GetComponent<Camera>();

        playerInput = GameObject.FindWithTag("Player")?.GetComponent<PlayerInput>() ?? null;

        player = GameObject.FindWithTag("Player").transform;

        prevPos =Vector2.zero; curPos=Vector2.zero;

        maxZdif = maxZ[0] - maxZ[1];
        minZdif = minZ[0] - minZ[1];
        maxXdif = maxX[0] - maxX[1];
        minXdif = minX[0] - minX[1];

        zoomPresent = 1 - ((cam.orthographicSize - minZoom) / (maxZoom - minZoom));

    }

    // Update is called once per frame
    void Update()
    {
        bool isPause = false;
        if (UIManager.Instance) isPause = UIManager.Instance.isPause;

        //マップの表示や非表示
        if (Input.GetKeyDown(KeyCode.M) && !isPause) {
            enable = !enable;
            foreach (GameObject child in children) { 
                child.SetActive(enable);
            }
            if (playerInput) playerInput.enabled = !enable;
            UIManager.Instance?.SetMouseVisible(enable);
            //mapCanvas.enabled = enable;
            if (player) {
                cam.transform.position = new Vector3(
                    player.position.x,
                    cam.transform.position.y,
                    player.position.z
                    );
                cam.transform.position = CheckMargin(cam.transform.position);
            }
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
            }
            if (Input.GetMouseButton(0)) {
                curPos = Input.mousePosition;
                if (Vector2.Distance(prevPos, curPos) != 0) { 
                    Vector2 dis = curPos- prevPos;
                    Vector3 pos = cam.transform.position;
                    pos.x += dis.y *(cam.orthographicSize/200) ;
                    pos.z -= dis.x * (cam.orthographicSize / 200);
                    cam.transform.position = CheckMargin(pos);
                    prevPos = curPos;
                }
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
}
