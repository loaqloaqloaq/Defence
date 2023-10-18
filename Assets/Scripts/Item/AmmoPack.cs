using UnityEngine;

public class AmmoPack : MonoBehaviour, IItem
{
    //追加弾数
    [SerializeField] private int ammo = 150;

    public void Use(GameObject target)
    {
        var player = target.GetComponent<ActiveWeapon>();
        var primaryWeapon = player.GetPrimaryWeapon();
        if (player != null && primaryWeapon != null)
        {
            primaryWeapon.ammoRemain += ammo;
            //追加した弾数は999を超えない
            if (primaryWeapon.ammoRemain + ammo > 999)
            {
                primaryWeapon.ammoRemain = 999;
            }
        }
        Destroy(gameObject);
    }
}