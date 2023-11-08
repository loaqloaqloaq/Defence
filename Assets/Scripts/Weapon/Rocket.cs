using UnityEngine;

public class Rocket : MonoBehaviour
{
    [SerializeField] private Transform attackRoot;
    [SerializeField] private float attackRadius;

    [SerializeField] private LayerMask whatIsTarget;

    [SerializeField] private float Speed = 350.0f;
    [SerializeField] private float rbForce = 6.0f; 

    private BoxCollider boxCollider;
    [SerializeField] private GameObject model;

    private Vector3 direction;

    private float duration = 10.0f;
    private float delay = 3.0f;

    private RaycastHit[] hits = new RaycastHit[10]; //攻撃時に接触したオブジェクトを配列で読み込む
    //private List<LivingEntity> lastAttackedTargets = new List<LivingEntity>(); //

    private float damageAmount;

    [SerializeField] ParticleSystem explosionVFX;

    private bool collide;

    private Ray ray;
    private RaycastHit hitInfo;

    Transform head;

    Vector3 start, end;

#if UNITY_EDITOR //unity editor内だけで動作する
    private void OnDrawGizmosSelected() //シーンでオブジェクトが選択された時に実行される関数
    {
        if (attackRoot != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(attackRoot.position, attackRadius); //オブジェクトの攻撃半径を球体に描画
        }
    }
#endif

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider>();
        Destroy(gameObject, duration);
        collide = false;
    }

    private void SetActive(bool isActive)
    {
        model.SetActive(isActive);
        boxCollider.enabled = isActive;
    }

    public void Initialize(Vector3 direction, float damage, Transform head)
    {
        this.direction = direction;
        this.head = head;
        damageAmount = damage;
        start = transform.position;
        end = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (collide) return;

        start = transform.position;
        Move();
        end = transform.position;
        RaycastSegment(start, end);
    }

    private void Move()
    {
        model.transform.LookAt(head);
        transform.Translate(direction * Speed * Time.deltaTime);
    }

    private void RaycastSegment(Vector3 start, Vector3 end)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;
        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            if (hitInfo.collider != null)
            {
                SetActive(false);
                collide = true;
                explosionVFX.Emit(1);
                SoundManager.Instance.Play("Sounds/Sfx/explosion1");
                Explosion();
            } 
        }
    }

    private void Explosion()
    {
        var colliders = Physics.OverlapSphere(attackRoot.position, attackRadius, whatIsTarget);

        //範囲内のTargetLayer Colliderにダメージ
        foreach (var collider in colliders)
        {
            var target = collider.GetComponent<IDamageable>();

            if (target != null)
            {
                Vector3 normal = collider.transform.position - transform.position;

                DamageMessage damageMessage;
                damageMessage.damager = null;
                damageMessage.amount = damageAmount;
                damageMessage.hitPoint = collider.transform.position;
                damageMessage.hitNormal = normal;

                target.ApplyDamage(damageMessage);
            }

            var rb = collider.GetComponent<Rigidbody>();
            
            if (rb) 
            {
                var dir = (collider.transform.position - attackRoot.position).normalized;
                //rb2d.AddForceAtPosition(dir * rbForce, collider.transform.position, ForceMode.Impulse);
                rb.AddForce(dir * rbForce, ForceMode.Impulse);
            }
        }
        Destroy(gameObject, delay);
    }
}

