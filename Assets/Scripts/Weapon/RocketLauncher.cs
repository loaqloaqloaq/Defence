using System;
using UnityEngine;

public class RocketLauncher : RaycastWeapon
{
    [SerializeField] GameObject rocket;

    public override void UpdateWeapon(float deltaTime)
    {
        isFiring = input.IsFiring || input.Fire;

        if (isFiring)
        {
            UpdateFiring(deltaTime);
        }
        else
        {
            accumulateTime = 0.0f; // << X ¼öÁ¤ ÇÊ¼E
            recoil.Reset();
        }
    }

    public override void Fire()
    {
        if (magAmmo <= 0)
        {
            //Reload
            var reload = input.GetComponent<ReloadWeapon>();
            if (reloadAvailable)
            {
                reload?.StartReload();
            }
            else
            {
                PlaySound("Empty");
            }
            return;
        }
        --magAmmo;
        PlaySound("Shot");                    //sound
        recoil.GernerateRecoil(weaponType);   //recoil

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
