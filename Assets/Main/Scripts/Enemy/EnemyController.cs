using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{      

    public void setType(int type) {
        int index = 0;
        type--;
        foreach (Transform child in transform)
        {           
            child.gameObject.SetActive(index == type);
            if (index == type) resetEnemy(child);
            index++;
        }
       
    }

    private void resetEnemy(Transform t) {
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
    }
}

public interface EnemyInterface {
    public void resetEnemy();
}


