using UnityEngine;
using System;
using TMPro;

//-------------------------------------------------
//敵拠点 (敵を生成する)
//-------------------------------------------------
public class enemyBase : MonoBehaviour, IDamageable
{
    private float HP;                //HP
    private float MaxHP;             //最大HP
    private float width, gaugeWidth; //ゲージ幅
    [SerializeField] GameObject canvas;      //敵拠点UI
    [SerializeField] GameObject explosion;   //爆発エフェクト
    [SerializeField] GameObject HPfill;      //HPゲージ
    [SerializeField] TextMeshProUGUI HPText; //HPテキスト
    [SerializeField] Animator animator;      //アニメーター  
    void Start()
    {
        //animatorを取得
        animator = GetComponent<Animator>();
        //初期値を設定
        gaugeWidth = 30.0f;
        width = gaugeWidth;
        MaxHP = 500f;
        HP = MaxHP;
        HPfill = transform.GetChild(0).GetChild(0).gameObject;
        HPfill.SetActive(true);
        HPText = transform.GetChild(0).GetChild(0).GetChild(1).GetComponent<TextMeshProUGUI>();
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";
    }
    void Update()
    {
        //HPが0以下になったとき
        if (HP <= 0)
        {
            IsDead();
        }
        //ゲージ幅が減ったとき
        if (Mathf.Abs(width - gaugeWidth) > 0.002f)
        {
            width += (gaugeWidth - width) * Time.deltaTime * 4f;
            HPfill.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(width, 2);
        }
        //常に敵拠点のHPをカメラの方向に向ける
        canvas.transform.forward = Camera.main.transform.forward;
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
        HPfill.GetComponent<Animator>().SetTrigger("hideHP");
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
}