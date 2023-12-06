using System.Collections.Generic;
using UnityEditor;
using UnityEditor.PackageManager;
using UnityEngine;

public class FlameThrower : RaycastWeapon
{
    [SerializeField] private GameObject firePref;

    [Range(0f, 1.0f)][SerializeField] private float fireDamageRatio = 0.5f;

    [Range(0.0f, 90.0f)][SerializeField] private float shootingAngle = 15.0f;

    private List<DamagerFire> fireList = new List<DamagerFire>();


    //攻撃範囲のデバッグ
    private void OnDrawGizmosSelected()
    {
        if (raycastOrigin != null)
        {
            var leftEyeRotation = Quaternion.AngleAxis(-shootingAngle * 0.5f, Vector3.up);

            var leftRayDirection = leftEyeRotation * raycastOrigin.forward;

            Handles.color = new Color(1f, 0.2f, 1f, 1.0f);
            Handles.DrawSolidArc(raycastOrigin.position, Vector3.up, leftRayDirection, shootingAngle, fireDistance);
        }
    }

    public override void Fire()
    {
        if (magAmmo <= 0)
        {
            PlaySound("Empty");
            return;
        }
        --magAmmo;
        PlaySound("Shot");                    //sound
        //recoil.GernerateRecoil(weaponName);   //recoil
        foreach (var particle in muzzleFlash) //effect
        {
            particle.Emit(1);
        }

        var colliders = Physics.OverlapSphere(raycastOrigin.position, fireDistance, whatIsTarget);

        foreach (var collider in colliders)
        {
            //文字化けしちゃった。なんで？
            if (!IsTargetOnShootingLine(collider.transform))
                continue;

            var colliderPart = collider.GetComponent<EnemyPart>() ?? null;
            if (colliderPart != null && colliderPart.GetPart() != Part.BODY) continue;

            var m_dF = collider.GetComponentInChildren<DamagerFire>();
 
            if (m_dF)
            {
                m_dF.ReNew();

                DamageMessage damageMessage;
                damageMessage.damager = weaponHolder;
                damageMessage.amount = damage;

                float yModifier = collider.bounds.size.y * 0.5f;
                damageMessage.hitPoint = collider.transform.position + new Vector3(0, yModifier, 0);
                damageMessage.hitNormal = collider.transform.position - transform.position; 
                damageMessage.attackType = AttackType.Fire;

                continue;
            }

            var target = collider.transform.GetComponent<IDamageable>();
            if (target != null)
            {
                DamagerFire targetFire = GetFire();

                ThrowFire(collider, target, targetFire);
            }
        }
    }

    private void ThrowFire(Collider collider, IDamageable target, DamagerFire fire)
    {
        fire.gameObject.SetActive(true);
        fire.SetTarget(target, weaponHolder);

        float yModifier = collider.bounds.size.y * 0.5f;
        fire.transform.position = collider.transform.position + new Vector3(0f, yModifier, 0f);
        fire.transform.parent = collider.transform;
    }

    private DamagerFire GetFire()
    {
        foreach (var fire in fireList)
        {
            if (!fire.gameObject.activeSelf)
            {
                return fire;
            }
        }
        var newfire = CreateFire();
        fireList.Add(newfire);

        return newfire;
    }

    private DamagerFire CreateFire()
    {
        if (!firePref)
        {
            Debug.Log("FirePref is NULL Reference");
            return null;
        }

        var obj = Instantiate(firePref, transform.position, Quaternion.identity, transform);
        var m_fD = obj.GetComponent<DamagerFire>();
        
        float fireDamage = damage * fireDamageRatio;
       
        m_fD.Initialize(transform, damage);
        return m_fD;
    }

    //敵が攻撃範囲内にいるかチェック
    private bool IsTargetOnShootingLine(Transform target)
    {
        if (!target) { return false; }

        var direction = target.position - raycastOrigin.position;

        float angle = shootingAngle;

        if (Vector3.Angle(direction, raycastOrigin.forward) > angle * 0.5f)
        {
            return false;
        }

        return true;
    }
}
