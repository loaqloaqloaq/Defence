using UnityEngine;

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
    //�C�e�̈ʒu
    private Vector3 cannonBall_Pos;
    //�C�e��Y�������̈ړ�
    private float ballmoveY;
    //�C�e�̍U����
    private int cannonBall_Damage;
    //�����G�t�F�N�g
    [SerializeField] private GameObject explosion;

    [SerializeField] private Collider cannonBall_Collider;

    void Start()
    {
        //�v���C���[���擾
        player = GameObject.Find(targetLayer);
        //�C�e����ɏ㏸������
        ballmoveY = 1.0f;
        //�C�e�̍U���͂�ݒ�
        cannonBall_Damage = 80;
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
        transform.Translate(0, ballmoveY, 0);

        //�C�e��20m�ȏ�A�㏸������
        if (transform.position.y >= 20.0f)
        {
            cannonBall_Pos = player.transform.position;
            cannonBall_Pos.y += 20.0f;
            //�C�e�̈ʒu��ύX
            transform.position = cannonBall_Pos;
            //�C�e�����ɍ~��������
            ballmoveY = -0.1f;
            //�����蔻���L��������
            cannonBall_Collider.enabled = true;
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
