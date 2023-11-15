using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;
using UnityEngine.Rendering;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject enemy;

    [SerializeField, ReadOnly]
    int EnemyCnt;

    float lastGen;
    public float genFreq;

    [SerializeField]
    TextAsset PatternJsonFile;
    string[] patterns;

    int[] currentLine;
    int currentLineIndex;
    int currentIndex;

    GameObject pool;

    int maxEnemy;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(enemy, transform.position, transform.rotation);        
        lastGen = 0;
        EnemyCnt = 0;

        Pattern enemyJson = JsonUtility.FromJson<Pattern>(PatternJsonFile.ToString());        
        patterns = enemyJson.pattern;

        genFreq = enemyJson.genFreq;
        maxEnemy = 100;

        pool = GameObject.Find("Enemy Pool");
        if (pool == null) pool = new GameObject("Enemy Pool");

        RandomNewPattern();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyCnt = 0;
        foreach (Transform e in pool.transform) {
            if (e.gameObject.activeSelf) {
                EnemyCnt++;
            }
        }
        if (EnemyCnt >= maxEnemy) return;

        lastGen += Time.deltaTime;
       
        if (lastGen >= genFreq) {
            lastGen = 0;
            int randX = Random.Range(-3, 3);
            int randZ = Random.Range(-3, 3);
            Vector3 pos = transform.position;
            pos = new Vector3(pos.x + randX, pos.y, pos.z + randZ);

            int type = currentLine[currentIndex];
            bool generated = false;

            foreach (Transform e in pool.transform)
            {
                if (!e.gameObject.activeSelf) {
                    generated = true;
                    e.gameObject.SetActive(true); 
                    e.transform.localPosition = pos;
                    e.GetComponent<EnemyController>().setType(type);
                    e.transform.SetParent(pool.transform, true);
                    break;
                }
            }
            if (!generated && pool.transform.childCount < maxEnemy) {
                GameObject e = Instantiate(enemy, pos, transform.rotation);
                e.GetComponent<EnemyController>().setType(type);
                e.transform.SetParent(pool.transform, true);
            }            
        }
    }

    private void RandomNewPattern()
    {
        currentLineIndex = Random.Range(0, patterns.Length);
        currentLine = patterns[currentLineIndex].Split(",").Select(i => int.Parse(i)).ToArray();
        currentIndex = 0;
    }
}
