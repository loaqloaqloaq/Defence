using UnityEditor.VersionControl;
using UnityEngine;

public class Mine : MonoBehaviour
{
    //敵1
    [SerializeField] GameObject Enemy1;
    //地雷の攻撃力
    private int mine_Damage;
    //死ぬまでのカウント
    private float damage_countDown;
    //死ぬ時間
    private float death_Time;
    void Start()
    {
        mine_Damage = 100; //仮ダメージ
        damage_countDown = 0.0f;
        death_Time = 100.0f;
    }

    //ダメージ発生カウント
    private void Damage_Count()
    {
        damage_countDown += Time.deltaTime;

        //10秒経ったら消滅
        if (damage_countDown >= death_Time)
        {
            Destroy(gameObject);
        }
    }

    //地雷に触れた時
    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        //ダメージ処理が発生したとき
        if (damageable != null)
        {
            //敵1が接触したとき
            if (collision.gameObject == Enemy1)
            {

                damageable.Damage(mine_Damage);

                Damage_Count();

                Debug.Log(collision.gameObject + "に" + mine_Damage + "ダメージを与える");
            }
        }
    }
}
