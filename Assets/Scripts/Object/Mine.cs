using UnityEditor.VersionControl;
using UnityEngine;

public class Mine : MonoBehaviour
{
    //�n���̍U����
    private int mine_Damage;
    //���ʂ܂ł̃J�E���g
    float damage_countDown;
    //���ʎ���
    float death_Time;
    void Start()
    {
        mine_Damage = 100; //���_���[�W
        damage_countDown = 0.0f;
        death_Time = 10.0f;
    }

    void Update()
    {

    }

    //�_���[�W�����J�E���g
    public void Damage()
    {
        
        damage_countDown += Time.deltaTime;

        //���b�o���������
        if (damage_countDown <= death_Time)
        {
            Destroy(gameObject);
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //if (collision.gameObject.name == "Player")
        //{
        //    var message = new DamageMessage();
        //    message.amount = mine_Damage; //�_���[�W
        //    message.damager = gameObject; //�U����
        //    Debug.Log("amout�F" + message.amount);
        //    Debug.Log("damager:" + message.damager);
        //}
    }
}
