using UnityEngine;

public class TurretSlot : MonoBehaviour
{
    [SerializeField] private Transform pivot;
    private EnemyGloable eg;

    public bool isTurretActive { get; private set; }

    private GameObject turret;

    private void Start()
    {
        isTurretActive = false;
        eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();
    }

    public void CreateTurret(GameObject turret)
    {
        if (isTurretActive) return;
        
        this.turret = Instantiate(turret, transform.position, Quaternion.identity, pivot);
        turret.transform.localPosition = Vector3.zero;
        isTurretActive = true;
        eg.TurretCreated(this.turret);
    }

    public void DestroyTurret()
    {
        Destroy(turret);
        isTurretActive = false;
        eg.TurretDestoried(this.turret);
    }

    public void ChangeTurret(GameObject turret)
    {
        DestroyTurret();
        CreateTurret(turret);
    }


}
