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

    float lastGen;
    float genFreq;
    float randomRange;
    float randomTime;

    [SerializeField]
    TextAsset PatternJsonFile;
    string[] patterns;

    int[] currentLine;
    int currentLineIndex;
    int currentIndex;

    GameObject pool;

    EnemyGloable eg;

    int maxEnemy;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(enemy, transform.position, transform.rotation);        
        lastGen = 0;
       

        Pattern enemyJson = JsonUtility.FromJson<Pattern>(PatternJsonFile.ToString());        
        patterns = enemyJson.pattern;

        genFreq = enemyJson.genFreq;
        randomRange = enemyJson.randomRange;
        maxEnemy = enemyJson.maxEnemy;

        randomTime = Random.Range(-randomRange, randomRange);

        pool = GameObject.Find("Enemy Pool");
        if (pool == null) pool = new GameObject("Enemy Pool");

        RandomNewPattern();

        eg= GameObject.Find("EnemyLoader").GetComponent<EnemyGloable>();
    }

    // Update is called once per frame
    void Update()
    {        
       

        lastGen += Time.deltaTime;       
        if (lastGen >= (genFreq + randomTime) ) {
            if (eg.enemyCnt >= maxEnemy) return;            
            lastGen = 0;
            randomTime = Random.Range(-randomRange, randomRange);
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
                e.transform.SetParent(pool.transform, true);
                e.GetComponent<EnemyController>().setType(type);
            }

            currentIndex++;
            if (currentIndex >= currentLine.Length) RandomNewPattern();
        }
    }

    private void RandomNewPattern()
    {
        currentLineIndex = Random.Range(0, patterns.Length);
        currentLine = patterns[currentLineIndex].Split(",").Select(i => int.Parse(i)).ToArray();
        currentIndex = 0;
    }
}
