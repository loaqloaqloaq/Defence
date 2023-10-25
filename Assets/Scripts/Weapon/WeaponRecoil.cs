using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponRecoil : MonoBehaviour
{
    [HideInInspector] public PlayerAiming characterAiming;
    [HideInInspector] public Cinemachine.CinemachineImpulseSource cameraShake;
    [HideInInspector] public Animator rigController;
    public float recoilModifier = 1.0f;

    [SerializeField] Vector2[] recoilPattern;
    [SerializeField] float duration;


    float verticalRecoil;
    float horizontalRecoil;
    float time;
    int index;

    public void Reset()
    {
        index = 0;
    }

    private void Awake()
    {
        cameraShake = GetComponent<Cinemachine.CinemachineImpulseSource>();
    }

    int NextIndex(int index)
    {
        return (index + 1) % recoilPattern.Length;
    }

    public void GernerateRecoil(string weaponName)
    {
        time = duration;

        cameraShake.GenerateImpulse(Camera.main.transform.forward);

        horizontalRecoil = recoilPattern[index].x;
        verticalRecoil = recoilPattern[index].y;

        index = NextIndex(index);
        Debug.Log(index);
        rigController.Play("weapon_recoil_" + weaponName, 1, 0.0f);
    }

    void Update()
    {
        if (time > 0)
        {
            characterAiming.yAxis.Value -= ((verticalRecoil / 1000) * Time.deltaTime / duration) * recoilModifier;
            characterAiming.xAxis.Value -= ((horizontalRecoil / 10) * Time.deltaTime / duration) * recoilModifier;
            time -= Time.deltaTime;
        }
    }
}
