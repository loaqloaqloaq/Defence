using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShopController : MonoBehaviour
{
    private Transform spawnPoint;
    private BoxCollider boxCollider;
    public int shopNumber;
    public Image iconImage;
    // Start is called before the first frame update
    void Awake()
    {
        spawnPoint = transform.GetChild(2);
    }

    private void Start()
    {
        boxCollider = GetComponent<BoxCollider>();
    }

    public Vector3 GetSpawnPoint() { 
        return spawnPoint.position;
    }

    //�G�̋��_���O�i���Ă����Ƃ��̏���
    public void BreakShop()
    {
        boxCollider.enabled = false;
        iconImage.color = new Color(1, 0, 0, 1);
    }
    
    //�G�̋��_����ނ����Ƃ�
    public void RevivalShop()
    {
        boxCollider.enabled = true;
        iconImage.color = new Color(1, 1, 1, 1);
    }
}
