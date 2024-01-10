using System;
using TMPro;
using UnityEngine;

//-------------------------------------------------
//敵拠点 (敵を生成する、砲弾を発射する)
//-------------------------------------------------
public class EnemyBase : MonoBehaviour, IDamageable
{
    private float HP;                //HP
    private float MaxHP;             //最大HP
    private float width, gaugeWidth; //ゲージ幅
    [SerializeField] GameObject canvas;      //敵拠点UI
    [SerializeField] GameObject explosion;   //爆発(砲弾発射) エフェクト
    [SerializeField] GameObject HPGauge;     //HPゲージ
    [SerializeField] TextMeshProUGUI HPText; //HPテキスト
    [SerializeField] Animator animator;      //アニメーター

    //ダメージを受けたかの確認
    private bool applydamage;
    //HPを回復する時間
    private float healTime;
    //砲弾発射カウント
    //private float firingCount;
    //砲弾発射頻度
    private float fireTime = 0f;
    private float fireDelay = 10.0f;
    //砲弾
    [SerializeField] GameObject cannonBall;
    [SerializeField] public bool fireCannon;

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
        //ダメージを受けていないを設定
        applydamage = false;
        healTime = 0.0f;
    }
    void Update()
    {
        //HPが0以下のとき
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

        //ダメージを受けた時
        if (applydamage == true)
        {
            //HPを回復
            HealHP();
        }

        if (fireCannon) { 
            //10秒に1回
            fireTime += Time.deltaTime;
            if (fireTime >= fireDelay)
            {
                //砲撃発射
                fireTime = 0;
                Cannonfiring();
            }
        }
    }

    //ダメージを受ける処理
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        //HPが0以下のとき
        if (HP <= 0)
        {
            return true;
        }
        //ダメージ処理
        else
        {
            //ダメージを食らう
            HP -= damageMessage.amount;
            //animationを再生
            animator.SetTrigger("damage");
            //ダメージを受けた
            applydamage = true;
        }
        //ゲージの更新
        gaugeWidth = 30f * HP / MaxHP;
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";

        return false;
    }

    //死んだ (破壊された) ときの処理
    public bool IsDead()
    {    
        //animationを再生
        animator.SetTrigger("break");
        HPGauge.GetComponent<Animator>().SetTrigger("hideHP");
        //爆発エフェクトの位置を設定
        Vector3 expPos = this.transform.position;
        expPos.y += 0.5f;
        //爆発エフェクト生成
        GameObject exp = Instantiate(explosion, expPos, this.transform.rotation);
        //大きさの設定
        exp.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        //オブジェクトを消滅        
        Destroy(gameObject);

        return true;
    }

    //HPを回復
    private void HealHP()
    {
        //HPを回復する時間を加算
        healTime += Time.deltaTime;

        //ダメージを受けてから10秒以上経過したら
        if (healTime >= 10.0f)
        {
            //HPを100回復
            HP += 100.0f;
            //時間をリセット
            healTime = 0.0f;
        }
        //HPが全回復したとき
        else if (HP >= MaxHP)
        {
            //HPを最初の値に戻す
            HP = MaxHP;
            //ダメージ受け状態をリセット
            applydamage = false;
        }
        //ゲージの更新
        gaugeWidth = 30f * HP / MaxHP;
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";
    }
    //砲撃発射
    private void Cannonfiring()
    {
        //弾丸発射エフェクトの位置を設定
        Vector3 firPos = this.transform.position;
        firPos.y += 0.9f;
        firPos.z += 11.0f;
        //弾丸発射エフェクトの生成
        GameObject shellfiring = Instantiate(explosion, firPos, Quaternion.identity);
        //大きさの設定
        shellfiring.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);
        //弾丸の位置を設定
        cannonBall.transform.position = shellfiring.transform.position;
        //砲弾生成
        var cb = Instantiate(cannonBall);
    }
}