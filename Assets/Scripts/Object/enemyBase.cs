using UnityEngine;
using System;
using TMPro;

public class enemyBase : MonoBehaviour, IDamageable
{
    private float HP;       //HP
    private float MaxHP;    //�ő�HP
    private float width, gaugeWidth; //�Q�[�W��
    
    [SerializeField]
    GameObject canvas;      //�G���_UI
    [SerializeField]
    GameObject explosion;   //�����G�t�F�N�g

    GameObject HPfill;      //HP�Q�[�W
    TextMeshProUGUI HPText; //HP�e�L�X�g
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
        //HP��0�ȉ��ɂȂ����Ƃ�
        if (HP <= 0)
        {
            Broke();
        }
        //�Q�[�W�����������Ƃ�
        if (Mathf.Abs(width - gaugeWidth) > 0.002f)
        {
            width += (gaugeWidth - width) * Time.deltaTime * 4f;
            HPfill.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(width, 2);
        }
        //��ɓG���_��HP���J�����̕����Ɍ�����
        canvas.transform.forward = Camera.main.transform.forward;
    }

    //�_���[�W���󂯂鏈��
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        if (HP <= 0)
        {
            return true;
        }
        //�_���[�W����
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

    //�j�󂳂ꂽ�Ƃ��̏���
    private void Broke()
    {
        GetComponent<BoxCollider>().enabled = false;
        animator.SetTrigger("break");
        HPfill.GetComponent<Animator>().SetTrigger("hideHP");
        //�����I�u�W�F�N�g�̐���
        var pos = transform.position;
        pos.y += 0.5f;
        var exp = Instantiate(explosion, pos, transform.rotation);
        exp.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        //�I�u�W�F�N�g��������
        Destroy(gameObject);
    }

    //�_���[�W����
    public void Damage(int damage)
    {
       //�Ȃ�
    }
}