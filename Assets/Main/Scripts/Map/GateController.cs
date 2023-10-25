using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.XR;


public class GateController : MonoBehaviour
{
    [HideInInspector]
    public float HP;
    private float MaxHP;
    private Animator ani;

    public int gateNumber;
    [HideInInspector]
    public bool broke;    

    NavMeshObstacle o1,o2;
  
    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 100f;
        HP = MaxHP;
        ani= GetComponent<Animator>();
        broke = false;
        o1 = transform.GetChild(0).GetChild(0).GetComponent<NavMeshObstacle>();
        
    }

    // Update is called once per frame
    void Update()
    {
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
        return true;
    }

    private void Broke(){
        broke = true;
        GetComponent<BoxCollider>().enabled = false;
        ani.SetTrigger("break");                
    }
}
