using System;
using TMPro;
using UnityEngine;

//-------------------------------------------------
//�G���_ (�G�𐶐�����A�C�e�𔭎˂���)
//-------------------------------------------------
public class EnemyBase : MonoBehaviour, IDamageable
{
    private float HP;                //HP
    private float MaxHP;             //�ő�HP
    private float width, gaugeWidth; //�Q�[�W��
    [SerializeField] GameObject canvas;      //�G���_UI
    [SerializeField] GameObject explosion;   //����(�C�e����) �G�t�F�N�g
    [SerializeField] GameObject HPGauge;     //HP�Q�[�W
    [SerializeField] TextMeshProUGUI HPText; //HP�e�L�X�g
    [SerializeField] Animator animator;      //�A�j���[�^�[

    //�_���[�W���󂯂����̊m�F
    private bool applydamage;
    //HP���񕜂��鎞��
    private float healTime;
    //�C�e���˃J�E���g
    //private float firingCount;
    //�C�e���˕p�x
    private float fireTime = 0f;
    private float fireDelay = 10.0f;
    //�C�e
    [SerializeField] GameObject cannonBall;
    [SerializeField] public bool fireCannon;

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
        //�_���[�W���󂯂Ă��Ȃ���ݒ�
        applydamage = false;
        healTime = 0.0f;
    }
    void Update()
    {
        //HP��0�ȉ��̂Ƃ�
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

        //�_���[�W���󂯂���
        if (applydamage == true)
        {
            //HP����
            HealHP();
        }

        if (fireCannon) { 
            //10�b��1��
            fireTime += Time.deltaTime;
            if (fireTime >= fireDelay)
            {
                //�C������
                fireTime = 0;
                Cannonfiring();
            }
        }
    }

    //�_���[�W���󂯂鏈��
    public bool ApplyDamage(DamageMessage damageMessage)
    {
        //HP��0�ȉ��̂Ƃ�
        if (HP <= 0)
        {
            return true;
        }
        //�_���[�W����
        else
        {
            //�_���[�W��H�炤
            HP -= damageMessage.amount;
            //animation���Đ�
            animator.SetTrigger("damage");
            //�_���[�W���󂯂�
            applydamage = true;
        }
        //�Q�[�W�̍X�V
        gaugeWidth = 30f * HP / MaxHP;
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";

        return false;
    }

    //���� (�j�󂳂ꂽ) �Ƃ��̏���
    public bool IsDead()
    {    
        //animation���Đ�
        animator.SetTrigger("break");
        HPGauge.GetComponent<Animator>().SetTrigger("hideHP");
        //�����G�t�F�N�g�̈ʒu��ݒ�
        Vector3 expPos = this.transform.position;
        expPos.y += 0.5f;
        //�����G�t�F�N�g����
        GameObject exp = Instantiate(explosion, expPos, this.transform.rotation);
        //�傫���̐ݒ�
        exp.transform.localScale = new Vector3(3.0f, 3.0f, 3.0f);
        //�I�u�W�F�N�g������        
        Destroy(gameObject);

        return true;
    }

    //HP����
    private void HealHP()
    {
        //HP���񕜂��鎞�Ԃ����Z
        healTime += Time.deltaTime;

        //�_���[�W���󂯂Ă���10�b�ȏ�o�߂�����
        if (healTime >= 10.0f)
        {
            //HP��100��
            HP += 100.0f;
            //���Ԃ����Z�b�g
            healTime = 0.0f;
        }
        //HP���S�񕜂����Ƃ�
        else if (HP >= MaxHP)
        {
            //HP���ŏ��̒l�ɖ߂�
            HP = MaxHP;
            //�_���[�W�󂯏�Ԃ����Z�b�g
            applydamage = false;
        }
        //�Q�[�W�̍X�V
        gaugeWidth = 30f * HP / MaxHP;
        HPText.text = HP + "/" + MaxHP + "(" + Math.Round(HP / MaxHP * 100, 2) + "%)";
    }
    //�C������
    private void Cannonfiring()
    {
        //�e�۔��˃G�t�F�N�g�̈ʒu��ݒ�
        Vector3 firPos = this.transform.position;
        firPos.y += 0.9f;
        firPos.z += 11.0f;
        //�e�۔��˃G�t�F�N�g�̐���
        GameObject shellfiring = Instantiate(explosion, firPos, Quaternion.identity);
        //�傫���̐ݒ�
        shellfiring.transform.localScale = new Vector3(4.0f, 4.0f, 4.0f);
        //�e�ۂ̈ʒu��ݒ�
        cannonBall.transform.position = shellfiring.transform.position;
        //�C�e����
        var cb = Instantiate(cannonBall);
    }
}