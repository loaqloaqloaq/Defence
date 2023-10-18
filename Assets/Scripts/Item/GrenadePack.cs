using UnityEngine;

public class GrenadePack : MonoBehaviour, IItem
{
    //グレネードの数を増やす
    public void Use(GameObject target)
    {
        var player = target.GetComponent<GrenadeController>();
        if (player != null)
        {
            player.AddGrenade();
        }
        Destroy(gameObject);
    }
}