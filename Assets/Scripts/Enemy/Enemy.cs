using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

#if UNITY_EDITOR //Only Unity Editor
using UnityEditor; 
#endif

public class Enemy : LivingEntity 
{
    //敵の状態 巡回,追跡,攻撃開始,攻撃中
    private enum State
    {
        Patrol,
        Tracking,
        AttackBegin,
        Attacking
    }
    [SerializeField] private State state;

    //コンポネント
    private NavMeshAgent agent;
    private Animator animator;
    private Renderer skinRenderer;

    [SerializeField] private Transform attackRoot; // 
    [SerializeField] private Transform eyeTransform; //視野の基準

    [SerializeField] private string hitSoundType;
    [SerializeField] private string deathSoundType;

    [Range(0.01f, 2f)] [SerializeField] private float turnSmoothTime = 0.1f; //回転の遅延時間
    private float turnSmoothVelocity; //回転の実時間変化量

    [SerializeField] private int startingScore = 100;
    [HideInInspector] public int currentScore;

    [SerializeField] private float damage = 30f;
    [SerializeField] private float attackRadius = 2f; //攻撃半径 
    [SerializeField] private float attackDistance; //攻撃可能な距離

    [SerializeField] private float fieldOfView = 50f; //視野角 
    [SerializeField] private float viewDistance = 10f; //視野の範囲 
    [SerializeField] private float patrolSpeed = 3f; //patrol状態の速度
    [SerializeField] private float runSpeed = 10f; //追跡状態の移動速度

    private LivingEntity targetEntity;
    [SerializeField] private LayerMask whatIsTarget;//攻撃対象をLayerMaskで特定


    private RaycastHit[] hits = new RaycastHit[10]; //攻撃時に接触したオブジェクトを配列で読み込む
    private List<LivingEntity> lastAttackedTargets = new List<LivingEntity>(); //

    private bool hasTarget => targetEntity != null && !targetEntity.dead; //いま追跡する対象が存在する＆死んでいない


#if UNITY_EDITOR //unity editor内だけで動作する
    private void OnDrawGizmosSelected() //シーンでオブジェクトが選択された時に実行される関数
    {
        if (attackRoot != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.5f);
            Gizmos.DrawSphere(attackRoot.position, attackRadius); //オブジェクトの攻撃半径を球体に描画
        }

        if (eyeTransform != null)
        {
            var leftEyeRotation = Quaternion.AngleAxis(-fieldOfView * 0.5f, Vector3.up);// vector3.upの基準になる軸はY軸
                                                                                        
            var leftRayDirection = leftEyeRotation * transform.forward;
            //現在見ている方向に合わせて視野をeditor上で描画する
            Handles.color = new Color(1f, 1f, 1f, 0.2f);
            Handles.DrawSolidArc(eyeTransform.position, Vector3.up, leftRayDirection, fieldOfView, viewDistance);
        }
    }
#endif

    //コンポネント取得＆初期化
    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        skinRenderer = GetComponentInChildren<Renderer>();

        //攻撃可能な距離を設定
        attackDistance = Vector3.Distance(transform.position,
                         new Vector3(attackRoot.position.x, transform.position.y, attackRoot.position.z)) +　attackRadius; 

        //攻撃するときに対象との距離
        agent.stoppingDistance = attackDistance;
    }

    // 初期設定
    public void Setup(float health, float damage,
        float runSpeed, float patrolSpeed, float waveIntensity, float animSpeed, Color skinColor) //EnemySpawner에 의해 호출 
    {
        this.startingHealth = health;
        this.health = health;

        this.damage = damage;
        this.runSpeed = runSpeed;
        this.patrolSpeed = patrolSpeed; //patrol speed = runSpeed * 0.3f
        this.currentScore = (int)(waveIntensity * startingScore);
        this.skinRenderer.material.color = skinColor;

        animator.speed = animSpeed;
        agent.speed = patrolSpeed;
    }

    //復活
    public void Respawn()
    {
        GetComponent<Collider>().enabled = true;
        targetEntity = null;
        agent.enabled = true;
        animator.applyRootMotion = false;
        state = State.Patrol;
        lastAttackedTargets.Clear();
        animator.Play("Enemy_Idle");
        StartCoroutine(UpdatePath());
    }

    private void Start()
    {
        //スクリプトでオブジェクトの回転処理
        agent.updateRotation = false;

        StartCoroutine(UpdatePath());
    }

    private void Update()
    {
        if (dead) return;

        if (state == State.Tracking && //追跡状態＆ターゲットが攻撃範囲内に入る 
            Vector3.Distance(targetEntity.transform.position, transform.position) <= attackDistance)
        {
            BeginAttack();　
        }

        //アニメーションの設定
        animator.SetFloat("Speed", agent.desiredVelocity.magnitude);
    }

    //状態に合わせてオブジェクトの行動を決める
    private void FixedUpdate()
    {
        if (dead) return;

        //パトロール&追撃時の回転処理
        if (state == State.Patrol || state == State.Tracking)
        {
            Vector2 forward = new Vector2(transform.position.z, transform.position.x);
            Vector2 steeringTarget = new Vector2(agent.steeringTarget.z, agent.steeringTarget.x);

            //方向を求めてAtan2を使って角度を求める
            Vector2 dir = steeringTarget - forward;
            float targetAngleY = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            targetAngleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY, //ターゲットがいる方向に回転する
                ref turnSmoothVelocity, turnSmoothTime);
            //角度適用
            transform.eulerAngles = Vector3.up * targetAngleY;        
        }

        if (targetEntity == null) return;

        //攻撃時の回転処理
        if (state == State.AttackBegin || state == State.Attacking) 
        {
            //Quaternion.LookRotation(TartgetPos - thisPos, Vector3.up) > look target, 2番目のパラメータは頭の方向
            var lookRotation = Quaternion.LookRotation(targetEntity.transform.position - transform.position, Vector3.up);
            var targetAngleY = lookRotation.eulerAngles.y; //
            
            targetAngleY = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngleY,
                ref turnSmoothVelocity, turnSmoothTime);
            //角度適用
            transform.eulerAngles = Vector3.up * targetAngleY; 
            //Vector3.up：(0, 1, 0) ＞ y軸 回転（左右）  
        }

        if (state == State.Attacking) 
        {　
            var direction = transform.forward;
            var deltaDistance = agent.velocity.magnitude * Time.deltaTime;

            var size = Physics.SphereCastNonAlloc(attackRoot.position, attackRadius, direction,
                hits, deltaDistance, whatIsTarget);　//SphereCastを通して読み込んだWhatIsTarget Layerのcolliderをhits[]に保存　

            for (var i = 0; i < size; i++)
            {
                var attackTargetEntity = hits[i].collider.GetComponent<LivingEntity>();
                //対象にダメージを与える処理
                if (attackTargetEntity != null && !lastAttackedTargets.Contains(attackTargetEntity))
                {
                    var message = new DamageMessage();
                    message.amount = damage;
                    message.damager = gameObject;

                    if (hits[i].distance <= 0f)
                    {
                        message.hitPoint = attackRoot.position;
                    }
                    else
                    {
                        message.hitPoint = hits[i].point;
                    }

                    message.hitNormal = hits[i].normal;
                    attackTargetEntity.ApplyDamage(message);
                    lastAttackedTargets.Add(attackTargetEntity); //FixedUpdateによって同じターゲットに重複攻撃するのを防止
                    break;
                }
            }
        }
    }

    //状態を更新＆ターゲットの更新
    private IEnumerator UpdatePath()  
    {
        while (!dead)
        {
            //ターゲットがいる
            if (hasTarget) 
            {
                //追跡状態に移る
                if (state == State.Patrol) 
                {
                    state = State.Tracking; 
                    agent.speed = runSpeed; 
                }
                agent.SetDestination(targetEntity.transform.position);
            }
            //ターゲットがない
            else
            {
                if (targetEntity != null) targetEntity = null; // ターゲットがいて死んでいたらtargetEntityを空ける

                //パトロール状態に移る
                if (state != State.Patrol)
                {
                    state = State.Patrol; 
                    agent.speed = patrolSpeed; 
                }

                //目的地にたどり着いたら新しい目的地を求めて更新
                if (agent.remainingDistance <= 2f) 
                {
                    var patrolTargetPosition
                        = Utility.GetRandomPointOnNavMesh(transform.position, 20f, NavMesh.AllAreas);
                    agent.SetDestination(patrolTargetPosition);
                }

                //OverlapSphereを通して仮想の球体と重なるすべてのcolliderを読み込む
                var colliders = Physics.OverlapSphere(eyeTransform.position, viewDistance, whatIsTarget);

                foreach (var collider in colliders)
                {
                    //オブジェクトが視野範囲内にない
                    if (!IsTargetOnSight(collider.transform))
                        continue; 
      
                    var livingEntity = collider.GetComponent<LivingEntity>();

                    //ターゲットを設定
                    if (livingEntity != null && !livingEntity.dead)
                    {
                        targetEntity = livingEntity;
                        break; 
                    }
                }
            }
            yield return new WaitForSeconds(0.03f); //更新周期 : 0.03秒
        }
    }

    //ダメージ受ける処理    
    public override bool ApplyDamage(DamageMessage damageMessage)
    {
        if (!base.ApplyDamage(damageMessage)) return false;

        if (targetEntity == null && damageMessage.damager != null)
        {
            targetEntity = damageMessage.damager.GetComponent<LivingEntity>();
        }

        EffectManager.Instance.PlayHitEffect(damageMessage.hitPoint, damageMessage.hitNormal,
            transform, EffectManager.EffectType.Flesh);

        SoundManager.Instance.Play("Sounds/Sfx/HitSound/" + hitSoundType);

        return true;
    }

    //攻撃開始
    public void BeginAttack()
    {
        state = State.AttackBegin;

        agent.isStopped = true;
        agent.velocity = Vector3.zero;
        animator.SetTrigger("Attack"); //攻撃アニメーション再生
    }

    //攻撃アニメーションから呼出される（始まるところ）
    public void EnableAttack() //Animator에 의해 호출되는 함수 
    {
        state = State.Attacking;

        lastAttackedTargets.Clear();
    }

    //攻撃アニメーションから呼出される（終わるところ）
    public void DisableAttack()
    {
        if (hasTarget)
        {
            state = State.Tracking;
        }
        else
        {
            state = State.Patrol;
        }

        agent.isStopped = false;
    }

    //オブジェクトとターゲットの間に障害物がいるか、視野範囲内にいるか確認
    private bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;

        var direction = target.position - eyeTransform.position;
        direction.y = eyeTransform.forward.y;

        //対象が視野から抜けてる
        if (Vector3.Angle(direction, eyeTransform.forward) > fieldOfView * 0.5f)
        {
            return false;
        }

        //オブジェクトと対象の間に障害物がないのを確認
        if (Physics.Raycast(eyeTransform.position, direction, out hit, viewDistance, whatIsTarget))
        {
            if (hit.transform == target)
                return true;
        }
        return false;
    }

    //死ぬ処理
    public override void Die()
    {
        base.Die();

        GetComponent<Collider>().enabled = false;
        agent.enabled = false;

        animator.applyRootMotion = true;
        animator.SetTrigger("Die");

        GameManager.Instance.AddKillCount();
        SoundManager.Instance.Play("Sounds/Sfx/HitSound/" + deathSoundType.ToString());
    }
}