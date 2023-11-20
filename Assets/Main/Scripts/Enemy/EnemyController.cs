using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public int type;
    private List<Transform> enemys = new List<Transform>();
    // Start is called before the first frame update
    void Start()
    {
        foreach (Transform child in transform)
        {
            enemys.Add(child);
            child.gameObject.SetActive(false);
        }        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void setType(int type) {
        enemys[type-1].gameObject.SetActive(true);
    }

    public void dead() {
        foreach (Transform child in transform)
        {            
            child.gameObject.SetActive(false);
        }
        gameObject.SetActive(false);
    }
}
