using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    int type;

    EnemyGloable eg;    
   
    private void Start()
    {        
        type = 0;   
        eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();
    }      

    public void setType(int type) {
        int index = 0;
        type--;
        this.type = type;
        foreach (Transform child in transform)
        {           
            
            if (index == type) {                
                resetEnemy(child);
                if(eg == null) eg = GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();
                eg.EnemyCreated();
            } 
            index++;
        }
       
    }

    private void resetEnemy(Transform t) {
        t.gameObject.SetActive(true);
        t.position = transform.position;
        EnemyInterface controller = t.GetComponent<EnemyInterface>();       
        controller.resetEnemy();
    }

    public void dead() {
        foreach (Transform child in transform)
        {            
            child.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
        eg.EnemyDestoried();
        GameManager.Instance.killCount++;
    }
}

public interface EnemyInterface {
    public void resetEnemy();
}


