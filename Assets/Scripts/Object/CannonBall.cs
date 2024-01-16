using UnityEngine;

//-------------------------------------------------
//�C�e (�v���C���[�̓���ɗ������ă_���[�W��^����)
//-------------------------------------------------
public class CannonBall : MonoBehaviour
{
    //�W�I��Layer (�v���C���[)
    [SerializeField] private string targetLayer;
    //�n�ʂ�Layer ()
    [SerializeField] private string Floor;
    //�v���C���[ (�ʒu�擾�p)
    [SerializeField] private GameObject player;
    //�C�e�̈ʒu��� (�������ꂽ�Ƃ��̍��W)
    private Vector3 cannonBall_Pos;
    //�C�e��Y�������̈ړ�
    private float ballmoveY;
    //�C�e�̍U����
    private int cannonBall_Damage;
    //�����G�t�F�N�g
    [SerializeField] private GameObject explosion;
    //�C�e�̓����蔻��
    [SerializeField] private Collider cannonBall_Collider;
    //�C�e�̗����n�_�}�[�J�[
    [SerializeField] private GameObject marker;
    //�����n�_�擾�p���W
    private Vector3 fallpoint;
    //�ŏ��ɗ������u��
    private bool firstfall;

    void Start()
    {
        //�v���C���[���擾
        player = GameObject.Find(targetLayer);
        //�C�e����ɏ㏸������
        ballmoveY = 1.0f;
        //�C�e�̍U���͂�ݒ�
        cannonBall_Damage = 80;
        //�}�[�J�[�폜�p��null
        //mark = null;
        //�C�e�̗����n�_�̏����ݒ�
        fallpoint = Vector3.zero;
        //�������Ă��Ȃ�
        firstfall = false;
    }

   void Update()
    {
        //�C�e�̍s��
        Move();
    }

    //�C�e�̍s��
    private void Move()
    {
        //�C�e�̈ړ�
        this.transform.Translate(0, ballmoveY, 0);

        //�C�e��30m�ȏ�A�㏸������
        if (this.transform.position.y >= 30.0f && !firstfall)
        {        
            //�C�e�̈ʒu��ݒ� (�v���C���[��20m��)           
            cannonBall_Pos = player.transform.position + new Vector3(0.0f, 29.0f, 0.0f);
            //�C�e�̈ʒu���X�V
            this.transform.position = cannonBall_Pos;
            //�����n�_���v���C���[�̈ʒu�ɐݒ�
            firstfall = true;
            //�����|�C���g�Ƀ}�[�J�[��\��
            FallpointMarker();
        }

        if (firstfall)
        {
            //�C�e�����ɍ~��������
            ballmoveY = -9.8f *Time.deltaTime;
            //�����蔻���L��������
            cannonBall_Collider.enabled = true;
            //�����J�n
            firstfall = true;
        }
    }
    //�����|�C���g�Ƀ}�[�J�[��\��
    public void FallpointMarker()
    {
        fallpoint = player.transform.position;
    }

    //���� (�ڐG)���N�������̏���
    private void Explotion()
    {
        //�����G�t�F�N�g�̈ʒu��ݒ�
        Vector3 expPos = this.transform.position;
        //�����G�t�F�N�g����
        GameObject exp = Instantiate(explosion, expPos, this.transform.rotation);
        //�傫���̐ݒ�
        exp.transform.localScale = new Vector3(2.0f, 2.0f, 2.0f);
        //�}�[�J�[�����ł�����
        //Destroy(mark);
        //�C�e�����ł�����
        Destroy(this.gameObject);
    }

    //�C�e�ɐG�ꂽ��
    void OnCollisionEnter(Collision collision)
    {
        //��������layer���擾
        string hitLayer = LayerMask.LayerToName(collision.gameObject.layer);

        //�v���C���[�ƐڐG�����Ƃ�
        if (hitLayer == targetLayer)
        {
            //�v���C���[�փ_���[�W�������s��
            DamageMessage damageMessage = new DamageMessage();
            damageMessage.damager = this.gameObject;
            damageMessage.amount = cannonBall_Damage;
            damageMessage.hitPoint = collision.transform.position;
            damageMessage.hitNormal = collision.transform.position - transform.position;
            player.GetComponent<PlayerHealth>().ApplyDamage(damageMessage);
        }
        //�����𔭐�������
        Explotion();
    }
}
