using UnityEngine;

public class HealthPack : MonoBehaviour, IItem
{
    [SerializeField] private float health = 50;

    //体力回復
    public void Use(GameObject target)
    {
        var livingEntity = target.GetComponent<LivingEntity>();

        if (livingEntity != null)
        {
            livingEntity.RestoreHealth(health);
        }

        Destroy(gameObject);
    }
}