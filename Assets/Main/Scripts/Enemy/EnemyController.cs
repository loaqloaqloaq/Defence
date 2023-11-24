using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    int type;  

    float offset = 50;
    private void Start()
    {        
        type = 0;        
    }  
    /*
    public void setRenders(bool isOffScreen) {
        var e = transform.GetChild(type);
        if (e.GetComponent<Renderer>()) e.GetComponent<Renderer>().forceRenderingOff = !isOffScreen;
        foreach (Renderer r in e.GetComponentsInChildren<Renderer>())
        {
            r.forceRenderingOff = !isOffScreen;
        }
    }
    */

    public void setType(int type) {
        int index = 0;
        type--;
        this.type = type;
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


