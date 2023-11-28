using System;
using TMPro;
using UnityEngine;

//-------------------------------------------------
//�G���_ (�G�𐶐�����)
//-------------------------------------------------
public class enemyBase : MonoBehaviour, IDamageable
{
    private float HP;                //HP
    private float MaxHP;             //�ő�HP
    private float width, gaugeWidth; //�Q�[�W��
    [SerializeField] GameObject canvas;      //�G���_UI
    [SerializeField] GameObject explosion;   //�����G�t�F�N�g
    [SerializeField] GameObject HPGauge;     //HP�Q�[�W
    [SerializeField] TextMeshProUGUI HPText; //HP�e�L�X�g
    [SerializeField] Animator animator;      //�A�j���[�^�[

    //�U�����J�E���g����
    private float attackCount;
    //�U�������鎞��
    private float attackTime;
    void Start()
    {
        //animator���擾
        animator = GetComponent<Animator>();
        //�����l��ݒ�
        gaugeWidth = 30.0f;
        width = gaugeWidth;
        MaxHP = 500f;
        HP = MaxHP;
        //HP�\���ݒ�
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";

        attackCount = 0.0f;
        attackTime = 20.0f; //20�b�Ɉ��U������
    }
    void Update()
    {
        //HP��0�ȉ��ɂȂ����Ƃ�
        if (HP <= 0)
        {
            //���񂾂Ƃ��̏���
            IsDead();
        }
        //�Q�[�W�����������Ƃ�
        if (Mathf.Abs(width - gaugeWidth) > 0.002f)
        {
            width += (gaugeWidth - width) * Time.deltaTime * 4f;
            HPGauge.transform.GetChild(0).GetComponent<RectTransform>().sizeDelta = new Vector2(width, 2);
        }
        //��ɓG���_��HP���J�����̕����Ɍ�����
        canvas.transform.forward = Camera.main.transform.forward;


        //20�b�Ɉ��C�����s�� (������)
        if (attackTime <= attackCount)
        {
            //�C������
            cannonAttack();
        }
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
            animator.SetTrigger("damage");
        }
        gaugeWidth = 30f * HP / MaxHP;
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";

        return false;
    }

    //���� (�j�󂳂ꂽ) �Ƃ��̏���
    public bool IsDead()
    {
        GetComponent<BoxCollider>().enabled = false;
        animator.SetTrigger("break");
        HPGauge.GetComponent<Animator>().SetTrigger("hideHP");
        //�����I�u�W�F�N�g�̐���
        var pos = transform.position;
        pos.y += 0.5f;
        var exp = Instantiate(explosion, pos, transform.rotation);
        exp.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        //�I�u�W�F�N�g��������
        Destroy(gameObject);

        return true;
    }
    //�_���[�W����
    public void Damage(int damage)
    {
        //�Ȃ�
    }

    //�C������
    private void cannonAttack()
    {
        //�U�����Ԃ����Z�b�g
        attackCount = 0.0f;
    }
}