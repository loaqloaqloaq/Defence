using UnityEngine;

public class GrenadePack : MonoBehaviour, IItem
{
    //グレネードの数を増やす
    public void Use(GameObject target, bool fromShop = false)
    {
        var player = target.GetComponent<GrenadeController>();
        if (player != null)
        {
            player.AddGrenade();
        }
        if (!fromShop) Destroy(gameObject);
    }
}