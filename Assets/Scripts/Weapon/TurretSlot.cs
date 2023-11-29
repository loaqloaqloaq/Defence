using UnityEngine;

public class TurretSlot : MonoBehaviour
{
    [SerializeField] private Transform pivot;

    public bool isTurretActive { get; private set; }

    private GameObject turret;

    private void Start()
    {
        isTurretActive = false;
    }

    public void CreateTurret(GameObject turret)
    {
        if (isTurretActive) return;
        
        this.turret = Instantiate(turret, transform.position, Quaternion.identity, pivot);
        turret.transform.localPosition = Vector3.zero;
        isTurretActive = true; 
    }

    public void DestroyTurret()
    {
        Destroy(turret);
        isTurretActive = false;
    }

    public void ChangeTurret(GameObject turret)
    {
        DestroyTurret();
        CreateTurret(turret);
    }


}
