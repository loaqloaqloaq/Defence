using Unity.VisualScripting;
using UnityEngine;


public class GateController : MonoBehaviour
{
    [HideInInspector]
    public float HP;
    private float MaxHP;
    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 100f;
        HP = MaxHP;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Damage(int atk)
    {
        HP -= atk;
        if (HP == 0) Broke();
    }
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        HP -= damageMessage.amount;
        if (HP == 0) Broke();
        return true;
    }

    private void Broke(){
        gameObject.transform.GetChild(0).gameObject.SetActive(false);
    }
}
