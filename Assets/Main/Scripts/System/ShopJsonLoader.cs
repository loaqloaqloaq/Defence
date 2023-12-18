using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopList
{
    public ShopItem[] items;
}

[Serializable]
public class ShopItem
{
    public string name;
    public int cost;
}

public class ShopJsonLoader : MonoBehaviour
{
    // Start is called before the first frame update

    //Singleton       
    private static ShopJsonLoader instance;
    public static ShopJsonLoader Instance
    {
        get
        {
            if (instance == null) instance = FindObjectOfType<ShopJsonLoader>();
            return instance;
        }
    }

    [SerializeField]
    public TextAsset ShopJsonFile;

    ShopItem[] items;
    public static ShopItem[] Items
    {
        get
        {
            return Instance.items;
        }
    }


    void Awake()
    {
        if (ShopJsonFile)
        {
            items = JsonUtility.FromJson<ShopList>(ShopJsonFile.ToString())?.items;            
        }
    }

}

