using UnityEngine;
using UnityEngine.UI;

//-------------------------------------------------
//�C�e (�v���C���[�̓���ɗ������Ă���)_��
//-------------------------------------------------
public class CannonBall : MonoBehaviour
{
    //�W�I��Layer (�v���C���[)
    [SerializeField] private string targetLayer;
    //�n�ʂ�Layer ()
    [SerializeField] private string Floor;
    //�v���C���[ (�ʒu�擾�p)
    [SerializeField] private GameObject player;
    //�v���C���[�̓��� (�}�[�J�[�̈ʒu�����p)
    //[SerializeField] PlayerInput playerinput;
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
    //�}�[�J�[�폜�p
    private GameObject mark;
    //�����n�_�擾�p���W
    private Vector3 fallpoint;
    //�ŏ��ɗ������u��
    private bool firstfall;

    void Start()
    {
        //�v���C���[���擾
        player = GameObject.Find(targetLayer);
        //�v���C���[�̓��͂��擾
        //playerinput = player.GetComponent<PlayerInput>();   
        //�C�e����ɏ㏸������
        ballmoveY = 1.0f;
        //�C�e�̍U���͂�ݒ�
        cannonBall_Damage = 80;
        //�}�[�J�[�폜�p��null
        mark = null;
        //�C�e�̗����n�_�̏����ݒ�
        fallpoint = Vector3.zero;
        //�������Ă��Ȃ�
        firstfall = false;
    }

   void Update()
    {
        //�C�e�̍s��
        Move();
        //�����|�C���g�Ƀ}�[�J�[��\��
        FallpointMarker();
    }

    //�C�e�̍s��
    private void Move()
    {
        //�C�e�̈ړ�
        this.transform.Translate(0, ballmoveY, 0);

        //�C�e��30m�ȏ�A�㏸������
        if (transform.position.y >= 30.0f)
        {
            //�C�e�̈ʒu��ݒ�
            cannonBall_Pos = player.transform.position;
            cannonBall_Pos.y += 20.0f;
            //�C�e�̈ʒu���X�V
            this.transform.position = cannonBall_Pos;
            //�C�e�����ɍ~��������
            ballmoveY = -0.1f;
            //�����蔻���L��������
            cannonBall_Collider.enabled = true;
            //�����J�n
            firstfall = true;
        }
    }
    //�����|�C���g�Ƀ}�[�J�[��\��
    private void FallpointMarker()
    {
        //�����J�n��
        if (firstfall == true)
        {
            //�����n�_���v���C���[�̈ʒu�ɂ���
            fallpoint = player.transform.position;
            //�����|�C���g�Ƀ}�[�J�[�𐶐�
            mark = Instantiate(marker, fallpoint, Quaternion.identity);
            //�傫���̐ݒ�
            mark.transform.localScale = new Vector3(2.5f, 0.001f, 2.5f);
            //���ɗ������Ă���
            firstfall = false;
        }
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
        Destroy(mark);
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
            DamageMessage damageable = new DamageMessage();
            damageable.damager = this.gameObject;
            damageable.amount = cannonBall_Damage;
            player.GetComponent<PlayerHealth>().ApplyDamage(damageable);
        }
        //�����𔭐�������
        Explotion();
    }
}
