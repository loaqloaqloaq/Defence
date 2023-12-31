using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyGenerator : MonoBehaviour
{    

    public void SpawnEnemy(int type,Transform e,GameObject pool) {
        int randX = Random.Range(-3, 3);
        int randZ = Random.Range(-3, 3);
        Vector3 pos = transform.position;
        pos.x += randX;
        pos.z += randZ;
        e.gameObject.SetActive(true);
        e.transform.localPosition = pos;
        e.GetComponent<EnemyController>().setType(type);
        e.transform.SetParent(pool.transform, true);
    }
    
}
