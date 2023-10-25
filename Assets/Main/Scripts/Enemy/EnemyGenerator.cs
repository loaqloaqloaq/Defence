using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject enemy;

    float lastGen;
    public float genFreq;
    // Start is called before the first frame update
    void Start()
    {
        Instantiate(enemy, transform.position, transform.rotation);        
        lastGen = 0;
    }

    // Update is called once per frame
    void Update()
    {
        lastGen += Time.deltaTime;
        int randX = Random.Range(-3, 3);
        int randZ = Random.Range(-3, 3);
        Vector3 pos = transform.position;
        pos = new Vector3(pos.x + randX, pos.y, pos.z + randZ);
        if (lastGen >= genFreq) {
            Instantiate(enemy, pos, transform.rotation);
            lastGen = 0;
        }
    }
}
