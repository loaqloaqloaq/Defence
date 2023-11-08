using System;
using UnityEngine;

public class RocketLauncher : RaycastWeapon
{
    [SerializeField] GameObject rocket;

    public override void UpdateWeapon(float deltaTime)
    {
        isFiring = input.isFiring || input.Fire;

        if (isFiring)
        {
            UpdateFiring(deltaTime);
        }
        else
        {
            accumulateTime = 0.0f; // << X ���� �ʼ�E
            recoil.Reset();
        }
    }

    public override void FireBullet()
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

        var direction = (raycastDestination.position - raycastOrigin.position).normalized;

        var rocketPref = Instantiate(rocket, raycastOrigin.position, Quaternion.identity);
        Rocket r = rocketPref.GetComponent<Rocket>();
        r.Initialize(direction, damage, raycastDestination, weaponHolder);
        //
    }

}
