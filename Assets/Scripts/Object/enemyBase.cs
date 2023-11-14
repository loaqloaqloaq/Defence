using UnityEngine;
using System;
using TMPro;

public class enemyBase : MonoBehaviour, IDamageable
{
    private float HP;       //HP
    private float MaxHP;    //最大HP
    private float width, gaugeWidth; //ゲージ幅
    
    [SerializeField]
    GameObject canvas;      //敵拠点UI
    [SerializeField]
    GameObject explosion;   //爆発エフェクト

    GameObject HPfill;      //HPゲージ
    TextMeshProUGUI HPText; //HPテキスト
    Animator animator;      //animatior
    void Start()
    {
        animator = GetComponent<Animator>();
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
            Broke();
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
            Debug.Log(HP);
            animator.SetTrigger("damage");
        }
        gaugeWidth = 30f * HP / MaxHP;
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";

        return false;
    }

    //破壊されたときの処理
    private void Broke()
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
    }

    //ダメージ処理
    public void Damage(int damage)
    {
       //なし
    }
}