using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        rigController.Play("weapon_recoil_" + weaponName, 1, 0.0f);
    }

    public void GernerateNPCRecoil(string weaponName)
    {
        time = duration;

        //cameraShake.GenerateImpulse(Camera.main.transform.forward);

        horizontalRecoil = 0;// recoilPattern[index].x;
        verticalRecoil = 0;// recoilPattern[index].y;

        index = NextIndex(index);

        //rigController.Play("weapon_recoil_" + weaponName, 1, 0.0f);
    }

    void Update()
    {
        if (time > 0)
        {
            if (characterAiming)
            {
                characterAiming.yAxis.Value -= ((verticalRecoil / 1000) * Time.deltaTime / duration) * recoilModifier;
                characterAiming.xAxis.Value -= ((horizontalRecoil / 10) * Time.deltaTime / duration) * recoilModifier;
            }
            time -= Time.deltaTime;
        }
    }
}
