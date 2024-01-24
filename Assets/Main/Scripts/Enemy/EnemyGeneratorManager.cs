using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemyGeneratorManager : MonoBehaviour
{
    static EnemyGeneratorManager instance;
    public static EnemyGeneratorManager Instance{
        get
        {
            if (!instance)
            {
                instance = FindObjectOfType<EnemyGeneratorManager>();
            }
            return instance;
        }
    }
    
    [SerializeField] List<GameObject> generators;

    [SerializeField] GameObject enemy;

    [SerializeField] public bool GenerateEnemy;

    float lastGen;
    float genFreq;
    float randomRange;
    float randomTime;

    [SerializeField] TextAsset PatternJsonFile;
    string[] patterns;

    int[] currentLine;
    int currentLineIndex;
    int currentIndex;

    GameObject pool;

    EnemyGloable eg;

    MaxEnemy[] max;
    [SerializeField]int maxEnemy;

    int currentGenerator;
    // Start is called before the first frame update
    void Start()
    {
        generators = GameObject.FindGameObjectsWithTag("EnemyGenerator").ToList();

        lastGen = 0;

        Pattern enemyJson = JsonUtility.FromJson<Pattern>(PatternJsonFile.ToString());
        patterns = enemyJson.pattern;

        genFreq = enemyJson.maxEnemy[0].genFreq;
        randomRange = enemyJson.randomRange;
        max = enemyJson.maxEnemy;

        maxEnemy = max[0].maxEnemy;

        randomTime = UnityEngine.Random.Range(-randomRange, randomRange);

        pool = GameObject.Find("Enemy Pool");
        if (pool == null) pool = new GameObject("Enemy Pool");

        RandomNewPattern();

        eg = GetComponent<EnemyGloable>();

        currentGenerator = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GenerateEnemy || generators.Count<=0) return;
        lastGen += Time.deltaTime;
        if (lastGen >= (genFreq + randomTime))
        {
            if (!generators[currentGenerator].activeSelf) return;
            if (eg.enemyCnt >= maxEnemy) return;
            lastGen = 0;
            randomTime = UnityEngine.Random.Range(-randomRange, randomRange);  

            int type = currentLine[currentIndex];
            bool generated = false;

            foreach (Transform e in pool.transform)
            {
                if (!e.gameObject.activeSelf)
                {
                    generated = true;
                    generators[currentGenerator].GetComponent<EnemyGenerator>().SpawnEnemy(type, e, pool);
                    break;
                }
            }
            if (!generated && pool.transform.childCount < maxEnemy)
            {
                GameObject e = Instantiate(enemy);
                e.transform.SetParent(pool.transform, true);                
                generators[currentGenerator].GetComponent<EnemyGenerator>().SpawnEnemy(type, e.transform, pool); 
            }
            currentGenerator++;            
            currentGenerator = currentGenerator % generators.Count;            
            currentIndex++;
            if (currentIndex >= currentLine.Length) RandomNewPattern();
        }
    }

    private void RandomNewPattern()
    {
        currentLineIndex = UnityEngine.Random.Range(0, patterns.Length);
        currentLine = patterns[currentLineIndex].Split(",").Select(i => int.Parse(i)).ToArray();
        currentIndex = 0;
    }

    public void ChangeMaxEnemy(float timeLeftPersent) {
        foreach (var m in max) {
            if (timeLeftPersent <= m.timeLeftPresent)
            {
                maxEnemy = m.maxEnemy;
                genFreq = m.genFreq;
            }
        }
    }
    public int UpdateGeneratorList( GameObject gen, int i = -1 ) {
        int index=-1;
        currentGenerator = 0;
        if (i==-1) {
            var exist = generators.FindIndex(g => g == gen);
            if(exist == -1) generators.Add(gen);
            index = generators.FindIndex(g => g == gen);
        }
        else {
            generators.RemoveAt(i);
            for(index = 0; index < generators.Count; index++){
                generators[index].GetComponent<EnemyGenerator>().index=index;
            }
        }
        
        //Debug.Log("Called" + (i == -1 ? "add" : "remove")+" "+generators.Count);
        return index;
    }



}
