//using UnityEditor.VersionControl;
using UnityEngine;

public class mine : MonoBehaviour
{
    //�n���̍U����
    private int mine_Damage;
    //�_���[�W�����J�E���g
    private float damage_countDown;
    //���ł��鎞��
    private float destroy_Time;
    //�ڐG����/���Ȃ�
    private bool contact;

    void Start()
    {
        mine_Damage = 100; //���_���[�W
        damage_countDown = 0.0f;
        destroy_Time = 20.0f;
        contact = false;
    }

    void Update()
    {
        //�ڐG�����玞�Ԃ��J�E���g
        if (contact == true)
        {
            damage_countDown += Time.deltaTime;
        }
        //�ڐG���Ă���20�b�o������n�������ł�����
        if (damage_countDown >= destroy_Time)
        {
            Destroy(this.gameObject);
        }
    }

    //�n���ɐG�ꂽ��
    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        string layerName = LayerMask.LayerToName(collision.gameObject.layer);

        //�_���[�W���������������Ƃ�
        if (damageable != null)
        {
            contact = true;
            //�G���ڐG�����Ƃ�
            if (layerName == "Enemy")
            {
                damageable.Damage(mine_Damage);

                Debug.Log(collision.gameObject + "��" + mine_Damage + "�_���[�W��^����");
            }
        }
    }
}
