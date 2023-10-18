using UnityEngine;


public class GateController : MonoBehaviour
{
    [HideInInspector]
    public int HP;
    private int MaxHP;
    // Start is called before the first frame update
    void Start()
    {
        MaxHP = 100;
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

    private void Broke(){
        Destroy(gameObject.transform.GetChild(0));
    }
}
