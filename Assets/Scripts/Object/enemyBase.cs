using System;
using TMPro;
using UnityEngine;

//-------------------------------------------------
//敵拠点 (敵を生成する)
//-------------------------------------------------
public class EnemyBase : MonoBehaviour, IDamageable
{
    private float HP;                //HP
    private float MaxHP;             //最大HP
    private float width, gaugeWidth; //ゲージ幅
    [SerializeField] GameObject canvas;      //敵拠点UI
    [SerializeField] GameObject explosion;   //爆発エフェクト
    [SerializeField] GameObject HPGauge;     //HPゲージ
    [SerializeField] TextMeshProUGUI HPText; //HPテキスト
    [SerializeField] Animator animator;      //アニメーター

    //攻撃後の時間をカウント
    private float afterAttackCount;
    //攻撃頻度
    private float attackTime;
    void Start()
    {
        //animatorを取得
        animator = GetComponent<Animator>();
        //初期値を設定
        gaugeWidth = 30.0f;
        width = gaugeWidth;
        MaxHP = 500f;
        HP = MaxHP;
        //HP表示設定
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";
        //攻撃後の時間をカウント
        afterAttackCount = 0.0f;
        //20秒に一回攻撃する
        attackTime = 20.0f;      
    }
    void Update()
    {
        //HPが0以下になったとき
        if (HP <= 0)
        {
            //死んだときの処理
            IsDead();
        }
        //ゲージ幅が減ったとき
        if (Mathf.Abs(width - gaugeWidth) > 0.002f)
        {
            width += (gaugeWidth - width) * Time.deltaTime * 4f;
            HPGauge.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(width, 2);
        }
        //常に敵拠点のHPをカメラの方向に向ける
        canvas.transform.forward = Camera.main.transform.forward;


        //20秒に一回砲撃を行う (未実装)
        if (afterAttackCount >= attackTime)
        {
            //砲撃処理
            cannonAttack();
        }
    }

    //ダメージを受ける処理
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        if (HP <= 0)
        {
            return true;
        }
        //ダメージ処理
        else
        {
            HP -= damageMessage.amount;
            animator.SetTrigger("damage");
        }
        gaugeWidth = 30f * HP / MaxHP;
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";

        return false;
    }

    //死んだ (破壊された) ときの処理
    public bool IsDead()
    {
        GetComponent<BoxCollider>().enabled = false;
        animator.SetTrigger("break");
        HPGauge.GetComponent<Animator>().SetTrigger("hideHP");
        //爆発オブジェクトの生成
        var pos = transform.position;
        pos.y += 0.5f;
        var exp = Instantiate(explosion, pos, transform.rotation);
        exp.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        //オブジェクトが消える
        Destroy(gameObject);

        return true;
    }
    //ダメージ処理
    public void Damage(int damage)
    {
        //なし
    }

    //砲撃処理
    private void cannonAttack()
    {

        //攻撃後から時間を計測
        afterAttackCount += Time.deltaTime;
        //攻撃時間をリセット
    }
}