using UnityEngine;

//-------------------------------------------
//�g�Q�̃g���b�v (�ʂ�ƃ_���[�W��^����]�[��)
//-------------------------------------------
public class Spears : MonoBehaviour
{
    //�W�I(�G) ��Layer
    [SerializeField] private string targetLayer = "Enemy";
    //�g�Q�̍U����
    [SerializeField] private int spearsDamage;
    //�g�Q�̈ړ����x
    private float spearsMoveSpeed;
    //�U���̃J�E���g
    private float AttackCount;
    //���Z�b�g�̃J�E���g
    private float ResetCount;
    //�g�Q�ōU�������鎞�� (5�b)
    [SerializeField] private float spearAttackTime;
    //�g�Q�����̈ʒu�ɖ߂����� (5�b)
    [SerializeField] private float spearResetTime;
    //���Z�b�g���̊m�F
    private bool reset = false;

    private void Start()
    {
        //�U���ƃ��Z�b�g�̎���
        spearAttackTime = 5.0f;
        spearResetTime = 5.0f;
        //�J�E���g�̏�����
        AttackCount = 0.0f;
        ResetCount = 0.0f;
        //���Z�b�g�ł͂Ȃ�
        reset = false;
    }
    private void Update()
    {
        //�U���̃J�E���g���s��
        AttackCount += Time.deltaTime;

        //�U���J�E���g���U�����Ԉȏォ���Z�b�g�łȂ��Ƃ�
        if (AttackCount >= spearAttackTime && !reset)
        {
            //�g�Q�̍U�� (�g�Q���n�ʂ����яo��)
            SpearsAttack();
        }
        //���Z�b�g�ɂȂ�����
        else if(reset == true)
        {
            //���Z�b�g�̃J�E���g���s��
            ResetCount += Time.deltaTime;
        }

        //�J�E���g�����Z�b�g���Ԉȏ�ɂȂ�����
        if (ResetCount >= spearResetTime)
        {
            //�g�Q�����̈ʒu�ɖ߂�
            SpearsReset();
        }
    }

    //�g�Q�̍U�� (�g�Q���n�ʂ����яo��)
    private void SpearsAttack()
    {
        //1.5m�ȏ�㏸����܂ł̊�
        if (transform.localPosition.y <= 1.5f)
        {
            //�g�Q�̑��x��ݒ� (�㏸���x)
            spearsMoveSpeed = 3.0f * Time.deltaTime;
            //�g�Q�̈ړ�
            this.transform.Translate(0, spearsMoveSpeed, 0);
        }
        else
        {
            //���Z�b�g�ɂ���
            reset = true;
        }
    }

    //�g�Q�����̈ʒu�ɖ߂�
    private void SpearsReset()
    {
        //0m�ȉ��ɂȂ�܂ł̊�
        if (transform.localPosition.y >= 0.0f)
        {
            //�g�Q�̑��x��ݒ� (�~�����x)
            spearsMoveSpeed = -3.0f * Time.deltaTime;
            //�g�Q�̈ړ�
            this.transform.Translate(0, spearsMoveSpeed, 0);
        }
        else
        {
            //�J�E���g�̏�����
            AttackCount = 0.0f;
            ResetCount = 0.0f;
            //���Z�b�g�̏�����
            reset = false;
        }
    }

    //�g�Q�ɐG��Ă����
    void OnCollisionStay(Collision collision)
    {
        //���������I�u�W�F�N�g�̃C���^�[�t�F�[�X���擾
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        //���������I�u�W�F�N�g��layer���擾
        string hitLayer = LayerMask.LayerToName(collision.gameObject.layer);

        //�G�ƐڐG�����Ƃ�
        if (hitLayer == targetLayer)
        {
            Debug.Log("�G�ɐG��Ă���");

            DamageMessage damageMessage = new DamageMessage();
            damageMessage.damager = gameObject;
            damageMessage.amount = spearsDamage;
            damageMessage.hitPoint = collision.transform.position;
            damageMessage.hitNormal = collision.transform.position - transform.position;
            //�G�փ_���[�W�������s��
            damageable.ApplyDamage(damageMessage);
        }
    }
}