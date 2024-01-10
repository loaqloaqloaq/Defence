using UnityEngine;

//-------------------------------------
//�n�� (�G�ꂽ��ꔭ�Ŏ���)
//-------------------------------------
public class Mine : MonoBehaviour
{
    //�n���̍U����
    [SerializeField] private int mine_Damage;
    //�_���[�W�����J�E���g
    private float damage_countDown;
    //���ł��鎞��
    private float destroy_Time;
    //�ڐG���� or ���Ȃ�
    private bool  contact;
    //�W�I(�G) ��Layer
    [SerializeField] private string targetLayer;

    void Start()
    {
        //�����l��ݒ�
        damage_countDown = 0.0f; //�_���[�W�����J�E���g
        destroy_Time = 20.0f;    //���ł��鎞��
        //�ڐG���Ă��Ȃ�
        contact = false;         
    }

    void Update()
    {
        //�ڐG�����玞�Ԃ��J�E���g
        if (contact == true)
        {
            damage_countDown += Time.deltaTime;
        }
        //�ڐG���Ă���20�b�o������
        if (damage_countDown >= destroy_Time)
        {
            //�n�������ł�����
            Destroy(this.gameObject);
        }
    }

    //�n���ɐG�ꂽ��
    void OnCollisionEnter(Collision collision)
    {
        //�C���^�[�t�F�[�X���擾
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        //��������layer���擾
        string hitLayer = LayerMask.LayerToName(collision.gameObject.layer);

        //�_���[�W���������������Ƃ�
        if (damageable != null)
        {
            //�ڐG����
            contact = true;

            //�G�ƐڐG�����Ƃ�
            if (hitLayer == targetLayer)
            {
                //�G�փ_���[�W�������s��
                DamageMessage damageMessage = new DamageMessage();
                damageMessage.damager = gameObject;
                damageMessage.amount = mine_Damage;
                damageMessage.hitPoint = collision.transform.position;
                damageMessage.hitNormal = collision.transform.position - transform.position;
                damageable.ApplyDamage(damageMessage);
            }
        }
    }
}
