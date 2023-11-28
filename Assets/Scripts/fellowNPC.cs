using System.Collections.Generic;
using UnityEditor.PackageManager;
using UnityEngine;
using UnityEngine.AI;

//-------------------------------------
//����(NPC) (���̂Ƃ��떳�G)
//-------------------------------------
public class fellowNPC : MonoBehaviour
{
    //����(NPC)�̏�� �ҋ@�A�ǐՁA�U��
    enum State
    {
        Idle,
        Tracking,
        Attack
    }
    //���݂̃X�e�[�g
    [SerializeField] private State fellowAI;

    //�W�I��Layer (��ԋ߂��̓G�𒲂ׂ邽�߂Ɏg��)
    [SerializeField] private string targetLayer;
    //�G�Ƃ̋���
    [SerializeField] private float enemyDistance;
    //�ǐՔ͈�
    [SerializeField] private float trackingRange;
    //�U���͈�
    [SerializeField] private float atkRange;
    //�G�Ƃ̋������� (�߂�����Ƃ�)
    [SerializeField] private float enemyDistanceLimit;
    //�ł������ɋ߂��G�I�u�W�F�N�g
    [SerializeField] private GameObject nearEnemyObj;
    //�G�֗^����_���[�W
    [SerializeField] private int attack;
    //�A�j���[�^�[
    [SerializeField] private Animator animator;
    //�U���\���̊m�F
    [SerializeField] private bool attackCheck;
    //������g���̂ɕK�v�ȃN���X
    [SerializeField] RaycastWeapon weapon;

    void Start()
    {
        //animator���擾
        animator = GetComponent<Animator>();
        //�ŏ��͑ҋ@���
        fellowAI = State.Idle;
        //�����l��ݒ�
        trackingRange = 20.0f;     //�ǐՔ͈�
        atkRange = 15.0f;          //�U���͈�
        enemyDistanceLimit = 1.0f; //�G�Ƃ̋������� (�߂�����Ƃ�)
        //�U����
        attack = 10;                
        weapon.damage = attack;
        //�U���̊m�F         
        attackCheck = false;
    }

    void Update()
    {
        //�G�̏��𒲂ׂ�
        enemySearch();
        //State�̕ύX
        changeState();

        //����(NPC)�̍s��
        switch (fellowAI)
        {
            //�ҋ@���
            case State.Idle:
                Idle();
                break;
            //�ǐՏ��
            case State.Tracking:
                Tracking();
                break;
            //�U�����
            case State.Attack:
                Attack();
                break;
        }
    }

    //�ҋ@��Ԃ̏���
    private void Idle()
    {
        //�U���s�\
        attackCheck = false;
        //�ǐՂ��Ă��Ȃ�
        animator.SetBool("Tracking", false);
        //�U�����Ă��Ȃ�
        animator.SetBool("Shooting", false);
    }
    //�ǐՏ�Ԃ̏���
    private void Tracking()
    {
        //�U���s�\
        attackCheck = false;
        //�U�����Ă��Ȃ�
        animator.SetBool("Shooting", false);
        //�ǐՂ��Ă���
        animator.SetBool("Tracking", true);
        //�^�[�Q�b�g(�G)�̍��W���擾
        Vector3 targetPos = nearEnemyObj.transform.position;
        //�^�[�Q�b�g��Y���W�������Ɠ����ɂ���2�����ɐ���
        targetPos.y = this.transform.position.y;
        //�^�[�Q�b�g(�G)�̕����֌�������
        transform.LookAt(targetPos);
    }
    //�U����Ԃ̏���
    private void Attack()
    {
        //�ǐ�
        Tracking();
        //�U���\�ɂ���
        attackCheck = true;
        //�e�������Ă��� (�U�����Ă���)
        animator.SetBool("Shooting", true);
        //�U��
        weapon.UpdateNPCWeapon(Time.deltaTime, attackCheck);
    }

    //�G�̏��𒲂ׂ�
    private void enemySearch()
    {
        //������1�ԋ����̋߂��G��T��
        nearEnemyObj = nearEnemySerch();

        //�G�����Ȃ��ꍇ
        if (nearEnemyObj == null)
        {
            Debug.Log("�G���t�B�[���h(�V�[��)�ɂ��܂���");
            return;
        }
        //�G�������ꍇ
        else if (nearEnemyObj == true)
        {
            //�G�Ǝ����̋������v�Z
            enemyDistance = Vector3.Distance(nearEnemyObj.transform.position, this.transform.position);
        }
    }

    //State�̕ύX
    private void changeState()
    {
        //�G�Ƃ̋������U���͈͓� (15�ȉ�)
        if (enemyDistance <= atkRange)
        {
            //�U����Ԃɂ���
            fellowAI = State.Attack;
        }
        //�G�Ƃ̋������U���͈͊O (15�ȏ�)
        if (enemyDistance >= atkRange)
        {
            //�ǐՏ�Ԃɂ���
            fellowAI = State.Tracking;
        }
        //�G�Ƃ̋������ǐՔ͈͊O (20�ȏ�)
        if (enemyDistance >= trackingRange)
        {
            //�ҋ@��Ԃɂ���
            fellowAI = State.Idle;
        }
        //�G�Ƃ̋������ǐՔ͈͊O (1�ȉ�)
        else if (enemyDistance <= enemyDistanceLimit)
        {
            //�ҋ@��Ԃɂ���
            fellowAI = State.Idle;
        }
    }

    //Enemy�^�O�̒��ōł��߂��I�u�W�F�N�g��1�擾
    private GameObject nearEnemySerch()
    {
        //�ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
        float nearDistance = 0;
        //�������ꂽ�ł��߂��Q�[���I�u�W�F�N�g�������邽�߂̕ϐ�
        GameObject searchTargetObj = null;
        //tagName�Ŏw�肳�ꂽTag�����A���ׂĂ̓G��z��Ɏ擾
        GameObject[] enemys = GameObject.FindGameObjectsWithTag(targetLayer);

        //�擾�����G��0�Ȃ�null��Ԃ�(�G���[��h������)
        if (enemys.Length == 0)
        {
            return searchTargetObj;
        }
        //���ׂĂ̓G����P����nearEnemy�ϐ��Ɏ��o��
        foreach (GameObject nearEnemy in enemys)
        {
            //nearEnemy�Ɏ��o�����G�ƁA����NPC(����)�Ƃ̋������v�Z���Ď擾
            float distance = Vector3.Distance(nearEnemy.transform.position, transform.position);
            //nearDistance��0(�ŏ��͂�����)�A��������nearDistance��distance�����傫���l�̏ꍇ
            if (nearDistance == 0 || nearDistance > distance)
            {
                //nearDistance���X�V
                nearDistance = distance;
                //searchTargetObj���X�V
                searchTargetObj = nearEnemy;
            }
        }
        //�ł��߂������I�u�W�F�N�g��Ԃ�
        return searchTargetObj;
    }
}
