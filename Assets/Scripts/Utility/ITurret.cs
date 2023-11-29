using UnityEngine;

public interface ITurret 
{
    public void CreateTurret(GameObject obj);
    
    public void DestroyTurret(GameObject obj);

    public void ChangeTurret(GameObject obj);
}
