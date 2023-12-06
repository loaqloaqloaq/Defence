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
        isFiring = input.IsFiring || input.Fire;

        if (isFiring && ReadyToFire)
        {
            fireTime = Time.time;
            Fire();
        }
        else
        {
            accumulateTime = 0.0f; // << X ���� �ʼ�E
            recoil.Reset();
        }

        CameraControl();

        UpdateBullets(deltaTime);
    }

    private void CameraControl()
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
