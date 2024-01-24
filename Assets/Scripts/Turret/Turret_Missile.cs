using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;
using UnityEditor;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class Turret_Missile : MonoBehaviour, Abnormality
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

    [SerializeField] private GameObject missilePref;

    public bool isInifinty;

    private bool isFiring;
    [SerializeField] float fireRate = 25;
    [SerializeField] int ammoRemain = 150;
    [SerializeField] int magAmmo;
    [SerializeField] int magCapacity = 30;
    [SerializeField] float damage = 10.0f;
    [Range(0.0f, 90.0f)][SerializeField] private float shootingAngle = 15.0f;

    [SerializeField] private Transform attackRoot;

    [SerializeField] private ParticleSystem[] muzzleFlash;
    [SerializeField] private ParticleSystem hitEffect;

    [SerializeField] private float reloadTime;
    [Range(0.0f, 5.0f)][SerializeField] private float turnSmoothVelocity = 3.0f;
    private float reloadDuration;

    [SerializeField] Transform firePivot;
    [SerializeField] Transform[] fireOrigin;
    //�U���͈�
    [SerializeField] private float attackRadius = 15.0f;
    [SerializeField] private float fieldOfView = 50f; //����p 

    //private RaycastHit[] hits = new RaycastHit[10]; //�U�����ɐڐG�����I�u�W�F�N�g��z��œǂݍ���

    [SerializeField] private LayerMask whatIsTarget;//�U���Ώۂ�LayerMask�œ���

    [SerializeField] private Transform target;

    [SerializeField] private AudioData fireSE;

    private float accumulateTime;

    private AudioSource audioSource;

    private List<HomingMissile> fireList = new List<HomingMissile>();

    [SerializeField]
    int[] abnormality = { 0, 0 };

    private bool IsTargetDead(Transform target)
    {
        var ITarget = target.GetComponent<IDamageable>();

        if (ITarget != null)
        {
            return ITarget.IsDead();
        }

        return true;
    }

#if UNITY_EDITOR //Turret�̍U���͈̓f�o�b�O
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

            Handles.color = new Color(1f, 0.2f, 1f, 0.15f);
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

        SoundManager.Instance?.AddAudioInfo(fireSE);

        abnormality = new int[] { 0, 0 };
    }

    public void Start()
    {
        damage = TurretJsonLoader.T3.atk;
    }

    private void FixedUpdate()
    {
        if (abnormality[(int)AbnormalityType.STOP] == 1) return;
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
        if (s == State.Reloading) { ReloadProcess(); return; }
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
        //if (target != null) target = null; // �^�[�Q�b�g�����Ď���ł�����targetEntity���󂯂�

        //OverlapSphere��ʂ��ĉ��z�̋��̂Əd�Ȃ邷�ׂĂ�collider��ǂݍ���
        var colliders = Physics.OverlapSphere(attackRoot.position, attackRadius, whatIsTarget);

        foreach (var collider in colliders)
        {
            //�I�u�W�F�N�g������͈͓��ɂȂ�
            if (!IsTargetOnSight(collider.transform))
                continue;

            target = collider.gameObject.transform;

            //�C��
            if (IsTargetDead(target))
                continue;

            //�^�[�Q�b�g��ݒ�
            if (target != null)
            {
                state = State.Attack;
                break;
            }
        }
    }

    //�I�u�W�F�N�g�ƃ^�[�Q�b�g�̊Ԃɏ�Q�������邩�A����͈͓��ɂ��邩�m�F
    private bool IsTargetOnSight(Transform target)
    {
        if (!target) { return false; }

        RaycastHit hit;

        var direction = target.position - attackRoot.position;
        //direction.y = raycastOrigin.forward.y;

        //�Ώۂ����삩�甲���Ă�
        if (Vector3.Angle(direction, attackRoot.forward) > fieldOfView * 0.5f)
        {
            return false;
        }

        //�I�u�W�F�N�g�ƑΏۂ̊Ԃɏ�Q�����Ȃ��̂��m�F
        if (Physics.Raycast(attackRoot.position, direction, out hit, attackRadius))
        {
            if (hit.transform == target)
                return true;
        }

        return false;
    }

    private void Rotate()
    {
        float yModifier = target.GetComponent<Collider>().bounds.size.y * 0.5f;

        Vector3 lookDirection = target.position + new Vector3(0f, yModifier, 0f) - firePivot.position;
        lookDirection.Normalize();

        firePivot.rotation = Quaternion.Slerp(firePivot.rotation, Quaternion.LookRotation(lookDirection), turnSmoothVelocity * Time.fixedDeltaTime);
    }

    private bool IsTargetOnShootingLine(Transform target)
    {
        if (!target) { return false; }

        var direction = target.position - attackRoot.position;
        //direction.y = raycastOrigin.forward.y;

        float angle = shootingAngle;

        //�Ώۂ����삩�甲���Ă�
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

    }

    public virtual void UpdateFiring(float deltaTime)
    {
        accumulateTime += deltaTime * 0.2f;
        float fireInterval = 1.0f / fireRate;
        while (accumulateTime >= 0.0f)
        {
            Fire();
            accumulateTime -= fireInterval;
        }
    }

    public virtual void Fire()
    {
        if (magAmmo <= 0)
        {
            reloadDuration = reloadTime;
            state = State.Reloading;
            return;
        }

        --magAmmo;
        PlaySound("Shot");                    //sound

        foreach(var origin in fireOrigin)
        {
            HomingMissile targetFire = GetFire();
            targetFire.Activate(origin.position, target);
        }

        foreach (var particle in muzzleFlash) //effect
        {
            particle.Emit(1);
        };

    }

    private HomingMissile GetFire()
    {
        foreach (var fire in fireList)
        {
            if (!fire.gameObject.GetComponent<HomingMissile>().isSimulating)
            {
                return fire;
            }
        }
        var newfire = CreateFire();
        fireList.Add(newfire);

        return newfire;
    }

    private HomingMissile CreateFire()
    {
        if (!missilePref)
        {
            Debug.Log("FirePref is NULL Reference");
            return null;
        }

        var obj = Instantiate(missilePref, transform.position, Quaternion.identity, transform);
        var m_fD = obj.GetComponent<HomingMissile>();
        m_fD.Initialize(damage);
        return m_fD;
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