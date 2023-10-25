using Unity.VisualScripting;
using UnityEngine;
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
    
    public GameObject explosion;
    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 100f;
        HP = MaxHP;
        ani= GetComponent<Animator>();
        broke = false;
    }

    // Update is called once per frame
    void Update()
    {

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
        var pos = transform.position;
        pos.y += 1.0f;
        if (explosion != null)
        {
            var exp=Instantiate(explosion, pos, transform.rotation);
            exp.transform.localScale = new Vector3(2,2,2);
        }
    }
}
