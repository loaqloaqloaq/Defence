﻿using UnityEngine;

public class AmmoPack : MonoBehaviour, IItem
{
    //追加弾数
    [SerializeField] private int ammo = 150;
    [SerializeField] ActiveWeapon.weaponSlot targetSlot;

    public void Use(GameObject target)
    {
        var player = target.GetComponent<ActiveWeapon>();
        var weapon = player.GetWeaponWithSlot(targetSlot);

        weapon?.AddAmmo(ammo);

        Destroy(gameObject);
    }
}