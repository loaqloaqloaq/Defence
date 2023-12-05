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
    [SerializeField] GameObject explosion;   //爆発(砲弾発射) エフェクト
    [SerializeField] GameObject HPGauge;     //HPゲージ
    [SerializeField] TextMeshProUGUI HPText; //HPテキスト
    [SerializeField] Animator animator;      //アニメーター

    //砲弾発射カウント
    private float firingCount;
    //砲弾発射頻度
    private float firingTime;
    //砲弾
    [SerializeField] GameObject cannonBall;

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
        //攻撃の時間のカウント
        firingCount = 0.0f;
        //5秒に一回攻撃する
        firingTime = 5.0f;
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

        //攻撃の時間のカウント
        firingCount += Time.deltaTime;

        //20秒に1回
        if (firingCount >= firingTime)
        {
            //砲撃発射
            cannonfiring();
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
            //ダメージを食らう
            HP -= damageMessage.amount;
            //animationを再生
            animator.SetTrigger("damage");
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

    //砲撃発射
    private void cannonfiring()
    {
        //弾丸発射エフェクトの位置を設定
        Vector3 firPos = this.transform.position;
        firPos.y += 0.9f;
        firPos.z += 11.0f;
        //弾丸発射エフェクトの生成
        GameObject shellfiring = Instantiate(explosion, firPos, Quaternion.identity);
        //大きさの設定
        shellfiring.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);

        //発射エフェクトと同じ場所に弾丸を生成
        cannonBall.transform.position = shellfiring.transform.position;
        //砲弾生成
        Instantiate(cannonBall);

        //砲弾発射カウントをリセット
        firingCount = 0.0f;
    }
}