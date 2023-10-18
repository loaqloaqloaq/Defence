using UnityEngine;

public class WeaponPickup : MonoBehaviour, IItem
{
    public RaycastWeapon weaponFab;

    public void Use(GameObject target)
    {
        ActiveWeapon activeWeapon = target.GetComponent<ActiveWeapon>();
        if (activeWeapon)
        {
            RaycastWeapon newWeapon = Instantiate(weaponFab);
            activeWeapon.Equip(newWeapon);
            Destroy(gameObject);
        }
    }
}
