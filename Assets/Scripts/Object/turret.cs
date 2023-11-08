using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class turret : MonoBehaviour
{
    //�^���b�g�̏�� �������Ȃ�,�ǐ�,�U��
    private enum State
    {
        Idle,
        Tracking,
        Attack
    }
    [SerializeField] private State state;     //���݂̃X�e�[�g

    //���g�Ƃ̋������v�Z����^�[�Q�b�g�I�u�W�F�N�g (�G)
    [SerializeField]
    private Transform targetObj;
    //�G�Ƃ̋���
    [SerializeField]
    private float enemyDistance;
    [SerializeField] private float trackingDistance = 10.0f; //�ǐՔ͈�
    [SerializeField] private float atkRange = 5.0f;          //�U���͈�
    [SerializeField] private float fieldOfView = 10.0f;      //����p 

    void Start()
    {
        state = State.Idle;
    }
    void Update()
    {
        enemyDistance = Vector3.Distance(targetObj.transform.position, this.transform.position);

        //State�̕ύX
        changeState();
        switch (state)
        {
            case State.Idle:
                Idle();
                break;
            case State.Tracking:
                Tracking();
                break;
            case State.Attack:
                Attack();
                break;
        }
    }

    //State�̕ύX
    private void changeState()
    {
        if (enemyDistance <= trackingDistance)
        {
            state = State.Tracking;
        }
        else
        {
            state = State.Idle;
        }
    }
    //�������Ȃ��Ƃ��̏���
    private void Idle()
    {
        this.transform.rotation = Quaternion.Euler(0.0f , 0.0f , 0.0f);
    }

    //�ǐՂ��Ă��鎞�̏���
    private void Tracking()
    {
        //����p�ɓG�������Ă������ԋ߂��̓G�̕���������
        if (enemyDistance <= fieldOfView)
        {
            Vector3 targetPos = targetObj.transform.position;
            // �^�[�Q�b�g��Y���W�������Ɠ����ɂ��邱�Ƃ�2�����ɐ�������B
            targetPos.y = this.transform.position.y;
            transform.LookAt(targetPos);
        }

        //��Object���������񂢂�Ƃ��̑Ώ��@������(1�̂̂�)

    }

    //�U�����Ă��鎞�̏���
    private void Attack()
    {
        //�e���΂�����
        if(enemyDistance <= atkRange)
        {

        }
    }
}
