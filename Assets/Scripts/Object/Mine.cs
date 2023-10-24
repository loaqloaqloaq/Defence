using UnityEditor.VersionControl;
using UnityEngine;

public class Mine : MonoBehaviour
{
    //地雷の攻撃力
    private int mine_Damage;
    //死ぬまでのカウント
    float damage_countDown;
    //死ぬ時間
    float death_Time;
    void Start()
    {
        mine_Damage = 100; //仮ダメージ
        damage_countDown = 0.0f;
        death_Time = 10.0f;
    }

    void Update()
    {

    }

    //ダメージ発生カウント
    public void Damage()
    {
        
        damage_countDown += Time.deltaTime;

        //数秒経ったら消滅
        if (damage_countDown <= death_Time)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.name == "Player")
        //{
        //    var message = new DamageMessage();
        //    message.amount = mine_Damage; //ダメージ
        //    message.damager = gameObject; //攻撃側
        //    Debug.Log("amout：" + message.amount);
        //    Debug.Log("damager:" + message.damager);
        //}
    }
}
