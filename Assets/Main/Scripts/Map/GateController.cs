using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using System.Xml.Linq;

public class GateController : MonoBehaviour,IDamageable
{
    [HideInInspector]
    public float HP;
    private float MaxHP;
    private Animator ani;

    public int gateNumber;
    [HideInInspector]
    public bool broke;    

    NavMeshObstacle o1;

    GameObject HPfill;
    TextMeshProUGUI HPText;  
    float width, gaugeWidth;

    private GameObject EnemyBase_Manager;
    private GameObject player;

    NavMeshObstacle o;


    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 100f;
        HP = MaxHP;
        ani= GetComponent<Animator>();
        broke = false;
        o1 = transform.GetChild(0).GetChild(0).GetComponent<NavMeshObstacle>();

        HPfill = transform.GetChild(3).GetChild(0).gameObject;
        HPfill.SetActive(false);
        HPText = transform.GetChild(3).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();

        HPText.text = (HP > 0 ? HP.ToString():"0") + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100,2) + "%)";        
        
        gaugeWidth = 30f;
        width = gaugeWidth;
        EnemyBase_Manager = GameObject.Find("EnemyBaseManager");

        player = GameObject.Find("Player");

        o = transform.Find("door/nk_group-3_LOD0-3_LOD0").GetComponent<NavMeshObstacle>();
    }

    // Update is called once per frame
    void Update()
    {

        if (HPfill.activeSelf){
            var lookPos = player.transform.position - transform.position;
            lookPos.y = 0;
            HPfill.transform.parent.rotation = Quaternion.LookRotation(lookPos) * Quaternion.Euler(0,180,0);           
        }
        if ( Mathf.Abs(width-gaugeWidth) > 0.002f) {
            width += (gaugeWidth - width)*Time.deltaTime*4f;
            HPfill.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(width, 2);
        }

        if (ani.GetCurrentAnimatorStateInfo(0).IsName("break") && ani.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f)
        {
            o1.enabled = false;           
        }
        else {
            o1.enabled = true;            
        }
    }
   
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        if (HP <= 0) return true;        
        HP -= damageMessage.amount;
        if (HP <= 0) Broke();
        else ani.SetTrigger("damage");

        HPfill.SetActive(true);
        gaugeWidth = 30f * HP / MaxHP;
        HPText.text = ( HP > 0 ? HP.ToString() : "0" )+ "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";


        return true;
    }

    private void Broke(){
        broke = true;
        GetComponent<BoxCollider>().enabled = false;
        ani.SetTrigger("break");
        HPfill.GetComponent<Animator>().SetTrigger("hideHP");
        if (EnemyBase_Manager != null && gateNumber <= 2)
        {
            EnemyBase_Manager.gameObject.GetComponent<EnemyBase_Manager>().stage[gateNumber - 1] = 1;
            EnemyBase_Manager.gameObject.GetComponent<EnemyBase_Manager>().teleportFlg = true;
        }
        else if( gateNumber >= 3)
        {
            Debug.Log("•‰‚¯");
            SceneManager.LoadScene("ResultLose");
        }
    }

    public bool IsDead()
    {
        return false;
    }
}
