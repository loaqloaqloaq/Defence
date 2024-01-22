using UnityEngine;

//アイテムインタフェース
public interface IItem　
{
    void Use(GameObject target,bool fromShop=false);
}