using System;
using UnityEngine;

public class SniperRifle : RaycastWeapon
{
    private PlayerAiming playerAiming;

    private bool isAiming;

    [SerializeField] private float timeBetFire = 1.7f;
    private float fireTime = 0f;

    private bool ReadyToFire { get { return fireTime + timeBetFire < Time.time; } }

    private void Start()
    {
        playerAiming = input.gameObject.GetComponent<PlayerAiming>();
    }

    public override void UpdateWeapon(float deltaTime)
    {
        isFiring = input.isFiring || input.Fire;
        Debug.Log(ReadyToFire);
        if (isFiring && ReadyToFire)
        {
            Debug.Log("fire");
            fireTime = Time.time;
            FireBullet();
        }
        else
        {
            accumulateTime = 0.0f; // << X ���� �ʼ�E
            recoil.Reset();
        }

        SnipingControl();

        UpdateBullets(deltaTime);
    }

    private void SnipingControl()
    {
        isAiming = input.zoom || input.Zoom >= 0.02f;
        if (isAiming)
        {
            playerAiming.OnSniping();
        }
        else
        {
            playerAiming.OffSniping();
        }
    }
}
