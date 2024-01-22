using UnityEngine;

public class WeaponPickup : MonoBehaviour, IItem
{
    public RaycastWeapon weaponFab;

    public void Use(GameObject target, bool fromShop = false)
    {
        ActiveWeapon activeWeapon = target.GetComponent<ActiveWeapon>();
        if (activeWeapon)
        {
            RaycastWeapon newWeapon = Instantiate(weaponFab);
            activeWeapon.Equip(newWeapon);
            if(!fromShop)Destroy(gameObject);
        }
    }
}
