using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;

public class Turret : MonoBehaviour, Abnormality
{
    enum State
    {
        StanBy,
        FindTarget,
        Reloading,
        Attack,
        Dead,
    }

    [SerializeField] private State state;

    public class Bullet
    {
        public bool isSimulating;
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
        public int bounce;
    }

    public bool isInifinty;

    private bool isFiring;
    [SerializeField] int fireRate = 25;
    [SerializeField] int ammoRemain = 150;
    [SerializeField] int magAmmo;
    [SerializeField] int magCapacity = 30;
    [SerializeField] float damage = 10.0f;
    [Range(0.0f, 90.0f)][SerializeField] private float shootingAngle = 15.0f;

    [SerializeField] private Transform raycastOrigin;
    private Transform raycastDestination;

    [SerializeField] private TrailRenderer tracerEffect;
    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private ParticleSystem hitEffect;

    [SerializeField] private float bulletSpeed = 1000.0f;
    [SerializeField] private float bulletDrop = 0.0f;
    [SerializeField] private int maxBounces = 0;
    [SerializeField] private float reloadTime;
    [Range(0.0f, 5.0f)][SerializeField] private float turnSmoothVelocity = 3.0f;
    private float reloadDuration;

    [SerializeField] Transform firePivot;

    //攻撃範囲
    [SerializeField] private float attackRadius = 15.0f;
    [SerializeField] private float fieldOfView = 50f; //視野角 

    //private RaycastHit[] hits = new RaycastHit[10]; //攻撃時に接触したオブジェクトを配列で読み込む

    [SerializeField] private LayerMask whatIsTarget;//攻撃対象をLayerMaskで特定

    List<Bullet> bullets = new List<Bullet>();

    private GameObject weaponHolder;

    [SerializeField] private Transform target;

    [Range(0.05f, 2.0f)][SerializeField] float bulletMaxLifeTime = 0.1f;

    [SerializeField] private AudioData fireSE;

    private AudioSource audioSource;

    [SerializeField]
    int[] abnormality = { 0, 0 };

    private bool IsTargetDead(Transform target)
    {
        var ITarget = target.GetComponent<IDamageable>();

        if (ITarget != null)
        {
            return ITarget.IsDead();
        }

        return false;
    }

#if UNITY_EDITOR //Turretの攻撃範囲デバッグ
    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
            Gizmos.DrawSphere(raycastOrigin.position, attackRadius);
        }

        if (raycastOrigin != null)
        {
            var leftEyeRotation = Quaternion.AngleAxis(-shootingAngle * 0.5f, Vector3.up);

            var leftRayDirection = leftEyeRotation * raycastOrigin.forward;

            Handles.color = new Color(1f, 0.2f, 1f, 1.0f);
            Handles.DrawSolidArc(raycastOrigin.position, Vector3.up, leftRayDirection, shootingAngle, attackRadius);
        }
    }
#endif

    public bool reloadAvailable
    {
        get
        {
            return ammoRemain > 0 && magAmmo < magCapacity || isInifinty; // 22 8 
        }
    }

    public virtual void Awake()
    {
        audioSource =GetComponent<AudioSource>();

        SoundManager.Instance?.AddAudioInfo(fireSE);

        abnormality = new int[] { 0, 0 };
    }

    private void FixedUpdate()
    {
        if (state == State.Attack) { Rotate(); }

        UpdateTurret(state);
    }


    private void UpdateTurret(State s)
    {
        if (abnormality[(int)AbnormalityType.STOP] == 1) return;
        if (s == State.StanBy) //switch
        {
            state = State.FindTarget;
            return;
        }
        if (s == State.Reloading)
        { ReloadProcess(); return; }
        if (s == State.FindTarget) { FindTarget(); return; }
        if (s == State.Reloading) { return; }

        if (s == State.Attack)
        {
            if (IsTargetDead(target) || !IsTargetOnSight(target))
            {
                state = State.StanBy;
                target = null;
            }

            UpdateWeapon(Time.deltaTime);
        }
    }

    private void ReloadProcess()
    {
        reloadDuration -= Time.deltaTime;

        if (reloadDuration < 0)
        {
            RefillAmmo();
            state = target != null ? State.Attack : State.StanBy;
        }
    }

    private void FindTarget()
    {
        //if (target != null) target = null; // ターゲットがいて死んでいたらtargetEntityを空ける

        //OverlapSphereを通して仮想の球体と重なるすべてのcolliderを読み込む
        var colliders = Physics.OverlapSphere(raycastOrigin.position, attackRadius, whatIsTarget);
       
        foreach (var collider in colliders)
        {
            //オブジェクトが視野範囲内にない
            if (!IsTargetOnSight(collider.transform))
                continue;

            target = collider.gameObject.transform;

            //修正
            if (IsTargetDead(target))
                continue;

            //ターゲットを設定
            if (target != null)
            {
                raycastDestination = target;
                state = State.Attack;
                break;
            }
        }
    }

    //オブジェクトとターゲットの間に障害物がいるか、視野範囲内にいるか確認
    private bool IsTargetOnSight(Transform target)
    {
        RaycastHit hit;

        var direction = target.position - raycastOrigin.position;
        //direction.y = raycastOrigin.forward.y;

        //対象が視野から抜けてる
        if (Vector3.Angle(direction, raycastOrigin.forward) > fieldOfView * 0.5f)
        {
            return false;
        }

        //オブジェクトと対象の間に障害物がないのを確認
        if (Physics.Raycast(raycastOrigin.position, direction, out hit, attackRadius))
        {
            if (hit.transform == target)
                return true;
        }

        return false;
    }

    private void Rotate()
    {
        Vector3 lookDirection = target.position - firePivot.position;
        lookDirection.Normalize();

        firePivot.rotation = Quaternion.Slerp(firePivot.rotation, Quaternion.LookRotation(lookDirection), turnSmoothVelocity * Time.fixedDeltaTime);
    } 

    private bool IsTargetOnShootingLine(Transform target)
    {
        if (!target) { return false; }

        var direction = target.position - raycastOrigin.position;
        //direction.y = raycastOrigin.forward.y;

        float angle = shootingAngle;

        //対象が視野から抜けてる
        if (Vector3.Angle(direction, raycastOrigin.forward) > angle * 0.5f)
        {
            return false;
        }

        return true;
    }

    public void OnEnable()
    {
        magAmmo = magCapacity;
    }


    public virtual Vector3 GetPosition(Bullet bullet)
    {
        // p + v * t + 0.5 * g * t * t
        Vector3 gravity = Vector3.down * bulletDrop;
        return bullet.initialPosition + bullet.initialVelocity * bullet.time + 0.5f * gravity * bullet.time * bullet.time;
    }

    public virtual Bullet CreateBullet(Vector3 position, Vector3 velocity)
    {
        Bullet bullet = new Bullet();
        bullet.isSimulating = true;
        bullet.initialPosition = position;
        bullet.initialVelocity = velocity;
        bullet.time = 0.0f;
        bullet.tracer = Instantiate(tracerEffect, position, Quaternion.identity);
        bullet.tracer.AddPosition(position);
        bullet.bounce = maxBounces;
        return bullet;
    }

    Ray ray;
    RaycastHit hitInfo;
    private float accumulateTime;

    public virtual void UpdateWeapon(float deltaTime)
    {
        isFiring = IsTargetOnShootingLine(target);

        if (isFiring) { UpdateFiring(deltaTime); }
        else { accumulateTime = 0.0f; } // << X ｼ､ ﾇﾊｼ・

        UpdateBullets(deltaTime);
    }

    public virtual void UpdateFiring(float deltaTime)
    {
        accumulateTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (accumulateTime >= 0.0f)
        {
            FireBullet();
            accumulateTime -= fireInterval;
        }
    }

    private void UpdateBullets(float deltaTime)
    {
        SimulateBullet(deltaTime);
    }

    private void SimulateBullet(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            if (bullet.time > bulletMaxLifeTime) // ﾀﾏﾁ､ ｽﾃｰ｣ﾀﾌ ﾁｪｸ・ﾀﾚｵｿﾀｸｷﾎ off
            {
                bullet.tracer.gameObject.SetActive(false);
                bullet.isSimulating = false;
                return;
            }
            if (!bullet.isSimulating) return;
            Vector3 p0 = GetPosition(bullet);
            bullet.time += deltaTime;
            Vector3 p1 = GetPosition(bullet);
            RaycastSegment(p0, p1, bullet);
        }
        );
    }

    private void RaycastSegment(Vector3 start, Vector3 end, Bullet bullet)
    {
        Vector3 direction = end - start;
        float distance = direction.magnitude;
        ray.origin = start;
        ray.direction = direction;

        if (Physics.Raycast(ray, out hitInfo, distance, whatIsTarget))
        {
            var target = hitInfo.collider.transform.GetComponent<IDamageable>();

            if (target != null)
            {
                DamageMessage damageMessage;
                damageMessage.damager = gameObject;
                damageMessage.amount = damage;
                damageMessage.hitPoint = hitInfo.point;
                damageMessage.hitNormal = hitInfo.normal;
                damageMessage.attackType = AttackType.Common;

                target.ApplyDamage(damageMessage);
            }
            else
            {
                //EffectManager.Instance.PlayHitEffect(hitInfo.point, hitInfo.normal, hitInfo.transform);
                hitEffect.transform.position = hitInfo.point;
                hitEffect.transform.forward = hitInfo.normal;
                hitEffect.Emit(1);
            }

            //bullet.tracer.transform.position = hitInfo.point;
            bullet.time = bulletMaxLifeTime;
            end = hitInfo.point;

            // Bullet ricochet
            if (bullet.bounce > 0)
            {
                bullet.time = 0;
                bullet.initialPosition = hitInfo.point;
                bullet.initialVelocity = Vector3.Reflect(bullet.initialVelocity, hitInfo.normal);
                bullet.bounce--;
            }
            else
            {
                bullet.tracer.gameObject.SetActive(false);
                bullet.isSimulating = false;
            }
        }
        else
        {
            bullet.tracer.transform.position = end;
        }
    }

    public virtual void FireBullet()
    {
        if (magAmmo <= 0)
        {
            reloadDuration = reloadTime;
            state = State.Reloading;
            return;
        }
        --magAmmo;
        PlaySound("Shot");                    //sound

        foreach (var particle in muzzleFlash) //effect
        {
            particle.Emit(1);
        }

        float yModifier = target.GetComponent<Collider>().bounds.size.y * 0.5f;
        var velocity = (raycastDestination.position + new Vector3(0f, yModifier, 0f) - raycastOrigin.position).normalized * bulletSpeed;

        foreach (var bullet in bullets)
        {
            if (!bullet.isSimulating)
            {
                bullet.isSimulating = true;
                bullet.tracer.gameObject.SetActive(true);
                bullet.time = 0;
                bullet.initialPosition = raycastOrigin.position;
                bullet.initialVelocity = velocity;
                return;
            }
        }
        var newBullet = CreateBullet(raycastOrigin.position, velocity);
        bullets.Add(newBullet);
    }

    public void PlaySound(string soundName)
    {
        switch (soundName)
        {
            //temp
            case "Shot":
                SoundManager.Instance?.PlaySE(fireSE.name, audioSource);
                break;

        }
    }
    public void RefillAmmo()
    {
        if (!reloadAvailable) { return; }
        if (isInifinty)
        {
            magAmmo = magCapacity;
            return;
        }
        int refillAmount = ammoRemain + magAmmo >= magCapacity ? magCapacity - magAmmo : ammoRemain;
        magAmmo += refillAmount;
        ammoRemain -= refillAmount;
    }

    public virtual void OnDestroy()
    {
        foreach (var bullet in bullets)
        {
            if (bullet.tracer != null)
            {
                Destroy(bullet.tracer.gameObject);
            }
        }
        bullets.Clear();
    }

    public void AddAbnormality(AbnormalityType at)
    {
        abnormality[(int)at] = 1;
    }

    public void RemoveAbnormality(AbnormalityType at)
    {
        abnormality[(int)at] = 0;
    }
    public int[] GetAbnormality()
    {
        return abnormality;
    }
}