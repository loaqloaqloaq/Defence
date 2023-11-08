using System.Collections.Generic;
using UnityEngine;

public class RaycastWeapon : MonoBehaviour
{
    protected PlayerInput input; 

     public class Bullet
    {
        public bool isSimulating;
        public float time;
        public Vector3 initialPosition;
        public Vector3 initialVelocity;
        public TrailRenderer tracer;
        public int bounce;
    }

    private enum SoundType
    { 
        shot,
        reload,
        empty,
        Max
    };

    public ActiveWeapon.weaponSlot weaponSlot;
    public WeaponRecoil recoil { get; private set; }
    public GameObject magazine;

    public bool isInifinty;

    public string weaponName;
    public string[] soundName = new string[(int)SoundType.Max];
    public bool isFiring;
    public int fireRate = 25;
    public int ammoRemain = 150;
    public int magAmmo;
    public int magCapacity = 30;
    public float damage = 20.0f;

    public Transform raycastOrigin;
    public Transform raycastDestination;

    [SerializeField] protected TrailRenderer tracerEffect;
    [SerializeField] protected ParticleSystem[] muzzleFlash;
    [SerializeField] protected ParticleSystem hitEffect;

    [SerializeField] protected float bulletSpeed = 1000.0f;
    [SerializeField] protected float bulletDrop = 0.0f;
    [SerializeField] protected int maxBounces = 0;

    List<Bullet> bullets = new List<Bullet>();

    protected GameObject weaponHolder;

    float maxLifeTime = 1.2f;

    public bool reloadAvailable
    {
        get
        {
            return ammoRemain > 0 && magAmmo < magCapacity || isInifinty; // 22 8 
        }
    }

    public virtual void Awake()
    {
        recoil = GetComponent<WeaponRecoil>();
    }

    public void OnEnable()
    {
        magAmmo = magCapacity;
    }

    public void SetHolder(GameObject holder, PlayerInput playerInput)
    {
        weaponHolder = holder;
        input = playerInput;
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
    protected float accumulateTime;

    public virtual void UpdateWeapon(float deltaTime)
    {
        isFiring = input.isFiring || input.Fire;

        if (isFiring)
        {
            UpdateFiring(deltaTime);
        }
        else
        {
            accumulateTime = 0.0f; // << X 수정 필펯E
            recoil.Reset();
        }
        UpdateBullets(deltaTime);
    }

    public virtual void UpdateNPCWeapon(float deltaTime,bool npcFiring)
    {
        isFiring = npcFiring;

        if (isFiring)
        {
            UpdateNPCFiring(deltaTime);
        }
        else
        {
            accumulateTime = 0.0f; // << X 수정 필펯E
            recoil.Reset();
        }
        UpdateBullets(deltaTime);
    }

    public virtual void StartFiring()
    {
        accumulateTime = 0.0f;
        recoil.Reset();
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
    public virtual void UpdateNPCFiring(float deltaTime)
    {
        accumulateTime += deltaTime;
        float fireInterval = 1.0f / fireRate;
        while (accumulateTime >= 0.0f)
        {
            NPCFireBullet();
            accumulateTime -= fireInterval;
        }
    }

    protected void UpdateBullets(float deltaTime)
    {
        SimulateBullet(deltaTime);
    }

    protected void SimulateBullet(float deltaTime)
    {
        bullets.ForEach(bullet =>
        {
            if (bullet.time > maxLifeTime) // 일정 시간이 지나툈E자동으로 off
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

        if (Physics.Raycast(ray, out hitInfo, distance))
        {
            var target = hitInfo.collider.transform.root.GetComponent<IDamageable>();

            if (target != null)
            {
                DamageMessage damageMessage;
                damageMessage.damager = weaponHolder;
                damageMessage.amount = damage;
                damageMessage.hitPoint = hitInfo.point;
                damageMessage.hitNormal = hitInfo.normal;

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
            bullet.time = maxLifeTime;
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

            // Collision Impulse
            var rb2d = hitInfo.collider.GetComponent<Rigidbody>();
            if (rb2d) { rb2d.AddForceAtPosition(ray.direction * 4, hitInfo.point, ForceMode.Impulse); }
            /*
            var enemyLivingEntity = hitInfo.collider.GetComponent<LivingEntity>();
            if (enemyLivingEntity)
            {
                enemyLivingEntity.Damaged(10.0f);
            }
            */
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
            PlaySound("Empty");
            return; 
        }
        --magAmmo;
        PlaySound("Shot");                    //sound
        recoil.GernerateRecoil(weaponName);   //recoil
        foreach (var particle in muzzleFlash) //effect
        {
            particle.Emit(1);
        }

        var velocity = (raycastDestination.position - raycastOrigin.position).normalized * bulletSpeed;

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
    public virtual void NPCFireBullet()
    {
        if (magAmmo <= 0)
        {
            RefillAmmo();
            return;
        }
        --magAmmo;
        PlaySound("Shot");                    //sound
        recoil.GernerateNPCRecoil(weaponName);   //recoil
        foreach (var particle in muzzleFlash) //effect
        {
            particle.Emit(1);
        }

        var velocity = transform.forward * bulletSpeed;

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
            case "Shot":
                SoundManager.Instance.Play("Sounds/Sfx/" + this.soundName[(int)SoundType.shot]);
                break;
            case "Reload":
                SoundManager.Instance.Play("Sounds/Sfx/" + this.soundName[(int)SoundType.reload]);
                break;
            case "Empty":
                SoundManager.Instance.Play("Sounds/Sfx/" + this.soundName[(int)SoundType.empty]);
                break;
            default:
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
}