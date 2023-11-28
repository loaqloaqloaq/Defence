using UnityEngine;

//-------------------------------------------------
//�^���b�g (�G�������ōU������)
//-------------------------------------------------
public class turret : MonoBehaviour
{
    //�^���b�g�̏�� �������Ȃ�,�ǐ�,�U��
    private enum State
    {
        Idle,
        Tracking,
        Attack
    }
    //���݂̃X�e�[�g
    [SerializeField] private State state;

    //�^���b�g�̏e��
    [SerializeField] private GameObject turret_muzzle;
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
    //�U���\���̊m�F
    [SerializeField] private bool attackCheck;
    //������g���̂ɕK�v�ȃN���X
    [SerializeField] private RaycastWeapon weapon;

    void Start()
    {
        //�ŏ��͑ҋ@���
        state = State.Idle;
        //�����l��ݒ�
        trackingRange = 20.0f;     //�ǐՔ͈�
        atkRange = 15.0f;          //�U���͈�
        enemyDistanceLimit = 2.0f; //�G�Ƃ̋������� (�߂�����Ƃ�)
        //�U����
        attack = 1;
        weapon.damage = attack;
        //�U���̊m�F         
        attackCheck = false;
    }
    void Update()
    {
        //�G�����邩���ׂ�
        nearEnemyObj = Serch();

        //�G�����Ȃ��Ƃ�
        if (nearEnemyObj == null)
        {
            //Debug.Log("�G���t�B�[���h��ɂ��܂���");
            return;
        }
        //�G�������ꍇ
        else if (nearEnemyObj == true)
        {
            //�G�Ƃ̋������v�Z
            enemyDistance = Vector3.Distance(nearEnemyObj.transform.position, this.transform.position);
        }

        //State�̕ύX
        changeState();

        //State���̍s��
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
        //�G�Ƃ̋������U���͈͓� (15�ȉ�)
        if (enemyDistance <= atkRange)
        {
            //�U������
            state = State.Attack;
        }
        //�G�Ƃ̋������U���͈͊O (15�ȏ�)
        if (enemyDistance >= atkRange)
        {
            //�ǐՂ���
            state = State.Tracking;
        }
        //�G�Ƃ̋������ǐՔ͈͊O (20�ȏ�)
        if (enemyDistance >= trackingRange)
        {
            //�������Ȃ�
            state = State.Idle;
        }
        else if(enemyDistance <= enemyDistanceLimit)
        {
            state = State.Idle;
        }
    }

    //�������Ȃ����̏���
    private void Idle()
    {
        //�U���s�\
        attackCheck = false;
    }
    //�ǐՂ��Ă��鎞�̏���
    private void Tracking()
    {
        //�U���s�\
        attackCheck = false;
        //�^�[�Q�b�g(�G)�̍��W���擾
        Vector3 targetPos = nearEnemyObj.transform.position;
        //�^�[�Q�b�g��Y���W�������Ɠ����ɂ��邱�Ƃ�2�����ɐ�������B
        targetPos.y = this.transform.position.y;
        //�^���b�g��G�֌�����
        transform.LookAt(targetPos);
        //�e���̌����������w��
        turret_muzzle.transform.LookAt(Serch().transform);
    }
    //�U�����Ă��鎞�̏���
    private void Attack()
    {
        //�ǐ�
        Tracking();
        //�U���\�ɂ���
        attackCheck = true;
        //�e���΂�����
        weapon.UpdateNPCWeapon(Time.deltaTime, attackCheck);
    }

    //��Object���������񂢂�Ƃ��̑Ώ��@ (1�̂̂�)
    //Enemy�^�O�̒��ōł��߂����̂��擾
    private GameObject Serch()
    {
        // �ł��߂��I�u�W�F�N�g�̋����������邽�߂̕ϐ�
        float nearDistance = 0;
        // �������ꂽ�ł��߂��Q�[���I�u�W�F�N�g�������邽�߂̕ϐ�
        GameObject searchTargetObj = null;
        // tagName�Ŏw�肳�ꂽTag�����A���ׂẴQ�[���I�u�W�F�N�g��z��Ɏ擾
        GameObject[] objs = GameObject.FindGameObjectsWithTag(targetLayer);

        // �擾�����Q�[���I�u�W�F�N�g�� 0 �Ȃ�null��߂�(�g�p����ꍇ�ɂ�null�ł��G���[�ɂȂ�Ȃ������ɂ��Ă���)
        if (objs.Length == 0)
        {
            return searchTargetObj;
        }

        // objs����P����obj�ϐ��Ɏ��o��
        foreach (GameObject obj in objs)
        {
            // obj�Ɏ��o�����Q�[���I�u�W�F�N�g�ƁA���̃Q�[���I�u�W�F�N�g�Ƃ̋������v�Z���Ď擾
            float distance = Vector3.Distance(obj.transform.position, transform.position);
            // nearDistance��0(�ŏ��͂�����)�A���邢��nearDistance��distance�����傫���l�Ȃ�
            if (nearDistance == 0 || nearDistance > distance)
            {
                // nearDistance���X�V
                nearDistance = distance;
                // searchTargetObj���X�V
                searchTargetObj = obj;
            }
        }
        //�ł��߂������I�u�W�F�N�g��Ԃ�
        return searchTargetObj;
    }
}