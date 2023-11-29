using UnityEngine;

public class TurretSlot : MonoBehaviour
{
    [SerializeField] private Transform pivot;

    [SerializeField] private bool isOn = false;

    private GameObject turret;

    public void CreateTurret(GameObject turret)
    {
        if (isOn) return;
        
        this.turret = Instantiate(turret, transform.position, Quaternion.identity, pivot);
        turret.transform.localPosition = Vector3.zero;
        isOn = true; 
    }

    public void DestroyTurret()
    {
        Destroy(turret);
        isOn = false;
    }

    public void ChangeTurret(GameObject turret)
    {
        DestroyTurret();
        CreateTurret(turret);
    }


}
