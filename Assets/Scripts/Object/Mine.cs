//using UnityEditor.VersionControl;
using UnityEngine;

public class mine : MonoBehaviour
{
    //地雷の攻撃力
    private int mine_Damage;
    //ダメージ発生カウント
    private float damage_countDown;
    //消滅する時間
    private float destroy_Time;
    //接触した/しない
    private bool contact;

    void Start()
    {
        mine_Damage = 100; //仮ダメージ
        damage_countDown = 0.0f;
        destroy_Time = 20.0f;
        contact = false;
    }

    void Update()
    {
        //接触したら時間をカウント
        if (contact == true)
        {
            damage_countDown += Time.deltaTime;
        }
        //接触してから20秒経ったら地雷を消滅させる
        if (damage_countDown >= destroy_Time)
        {
            Destroy(this.gameObject);
        }
    }

    //地雷に触れた時
    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        //ダメージ処理が発生したとき
        if (damageable != null)
        {
            contact = true;
            //敵が接触したとき
            if (layerName == "Enemy")
            {
                damageable.Damage(mine_Damage);

                Debug.Log(collision.gameObject + "に" + mine_Damage + "ダメージを与える");
            }
        }
    }
}
