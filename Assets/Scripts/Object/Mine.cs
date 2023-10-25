using UnityEditor.VersionControl;
using UnityEngine;

public class Mine : MonoBehaviour
{
    //�G1
    [SerializeField] GameObject Enemy1;
    //�n���̍U����
    private int mine_Damage;
    //���ʂ܂ł̃J�E���g
    private float damage_countDown;
    //���ʎ���
    private float death_Time;
    void Start()
    {
        mine_Damage = 100; //���_���[�W
        damage_countDown = 0.0f;
        death_Time = 100.0f;
    }

    //�_���[�W�����J�E���g
    private void Damage_Count()
    {
        damage_countDown += Time.deltaTime;

        //10�b�o���������
        if (damage_countDown >= death_Time)
        {
            Destroy(gameObject);
        }
    }

    //�n���ɐG�ꂽ��
    void OnCollisionEnter(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();

        //�_���[�W���������������Ƃ�
        if (damageable != null)
        {
            //�G1���ڐG�����Ƃ�
            if (collision.gameObject == Enemy1)
            {

                damageable.Damage(mine_Damage);

                Damage_Count();

                Debug.Log(collision.gameObject + "��" + mine_Damage + "�_���[�W��^����");
            }
        }
    }
}
