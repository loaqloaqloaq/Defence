using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class Turret_FlameThrower : MonoBehaviour
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

    public bool isInifinty;

    private bool isFiring;
    [SerializeField] int fireRate = 25;
    [SerializeField] int ammoRemain = 150;
    [SerializeField] int magAmmo;
    [SerializeField] int magCapacity = 30;
    [SerializeField] float damage = 10.0f;
    [Range(0.0f, 90.0f)][SerializeField] private float shootingAngle = 15.0f;

    [SerializeField] private Transform attackRoot;
    private Transform raycastDestination;

    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private ParticleSystem hitEffect;

    [SerializeField] private float reloadTime;
    [Range(0.0f, 5.0f)][SerializeField] private float turnSmoothVelocity = 3.0f;
    private float reloadDuration;

    [SerializeField] Transform firePivot;

    //攻撃範囲
    [SerializeField] private float attackRadius = 15.0f;
    [SerializeField] private float fieldOfView = 50f; //視野角 

    //private RaycastHit[] hits = new RaycastHit[10]; //攻撃時に接触したオブジェクトを配列で読み込む

    [SerializeField] private LayerMask whatIsTarget;//攻撃対象をLayerMaskで特定

    private GameObject weaponHolder;

    [SerializeField] private Transform target;

    [SerializeField] private AudioData fireSE;

    private float accumulateTime;

    private AudioSource audioSource;

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
        if (attackRoot != null)
        {
            Gizmos.color = new Color(1f, 0f, 0f, 0.15f);
            Gizmos.DrawSphere(attackRoot.position, attackRadius);
        }

        if (attackRoot != null)
        {
            var leftEyeRotation = Quaternion.AngleAxis(-shootingAngle * 0.5f, Vector3.up);

            var leftRayDirection = leftEyeRotation * attackRoot.forward;

            Handles.color = new Color(1f, 0.2f, 1f, 1.0f);
            Handles.DrawSolidArc(attackRoot.position, Vector3.up, leftRayDirection, shootingAngle, attackRadius);
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
        audioSource = GetComponent<AudioSource>();

        SoundManager.Instance.AddAudioInfo(fireSE);
    }

    private void FixedUpdate()
    {
        if (state == State.Attack) { Rotate(); }

        UpdateTurret(state);
    }


    private void UpdateTurret(State s)
    {
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
        var colliders = Physics.OverlapSphere(attackRoot.position, attackRadius, whatIsTarget);

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

        var direction = target.position - attackRoot.position;
        //direction.y = raycastOrigin.forward.y;

        //対象が視野から抜けてる
        if (Vector3.Angle(direction, attackRoot.forward) > fieldOfView * 0.5f)
        {
            return false;
        }

        //オブジェクトと対象の間に障害物がないのを確認
        if (Physics.Raycast(attackRoot.position, direction, out hit, attackRadius))
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

        var direction = target.position - attackRoot.position;
        //direction.y = raycastOrigin.forward.y;

        float angle = shootingAngle;

        //対象が視野から抜けてる
        if (Vector3.Angle(direction, attackRoot.forward) > angle * 0.5f)
        {
            return false;
        }

        return true;
    }

    public void OnEnable()
    {
        magAmmo = magCapacity;
    }

    public virtual void UpdateWeapon(float deltaTime)
    {
        isFiring = IsTargetOnShootingLine(target);

        if (isFiring) { UpdateFiring(deltaTime); }
        else { accumulateTime = 0.0f; } // << X ｼ､ ﾇﾊｼ・

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
        };
        
        //攻撃処理
        var colliders = Physics.OverlapSphere(attackRoot.position, attackRadius, whatIsTarget);

        foreach (var collider in colliders)
        {
            //オブジェクトが視野範囲内にない
            if (!IsTargetOnSight(collider.transform))
                continue;

            var target = collider.transform.GetComponent<IDamageable>();

            if (target != null)
            {
                DamageMessage damageMessage;
                damageMessage.damager = gameObject;
                damageMessage.amount = damage;
                damageMessage.hitPoint = Vector3.zero;
                damageMessage.hitNormal = Vector3.zero;

                target.ApplyDamage(damageMessage);
            }
        }
    }

    public void PlaySound(string soundName)
    {
        switch (soundName)
        {
            //temp
            case "Shot":
               // SoundManager.Instance.PlaySE(fireSE.name, audioSource);
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
}