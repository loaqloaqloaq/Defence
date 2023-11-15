using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Collections;
using UnityEngine;

public class EnemyGenerator : MonoBehaviour
{
    [SerializeField]
    GameObject[] enemy=new GameObject[5];

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

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(enemy, transform.position, transform.rotation);        
        lastGen = 0;
        EnemyCnt = 0;

        Pattern enemyJson = JsonUtility.FromJson<Pattern>(PatternJsonFile.ToString());        
        patterns = enemyJson.pattern;

        genFreq = enemyJson.genFreq;

        GameObject pool= GameObject.Find("Enemy Pool");
        if (pool == null) {
            pool = new GameObject("Enemy Pool");
        }
        for (int i = 1; i <= 5; i++) {
            if (GameObject.Find("Enemy Pool/Enemy" + i.ToString()) == null)
            {
                var tmp = new GameObject("Enemy" + i.ToString());
                tmp.transform.parent = pool.transform;
            }
        }  

        RandomNewPattern();
    }

    // Update is called once per frame
    void Update()
    {
        EnemyCnt = GameObject.FindGameObjectsWithTag("Enemy").Length;
        if (EnemyCnt > 100) return;
        lastGen += Time.deltaTime;
        int randX = Random.Range(-3, 3);
        int randZ = Random.Range(-3, 3);
        Vector3 pos = transform.position;
        pos = new Vector3(pos.x + randX, pos.y, pos.z + randZ);
        if (lastGen >= genFreq) {
            int type = currentLine[currentIndex]-1;
            //Debug.Log(type);
            if (type >= 0 && type < enemy.Length)
                try
                {
                    var e = Instantiate(enemy[type], pos, transform.rotation);
                    e.transform.parent = GameObject.Find("Enemy Pool/Enemy"+ currentLine[currentIndex].ToString()).transform;
                }
                catch (System.Exception e) {
                    Debug.Log(e.ToString());
                    Debug.LogError("line " + currentLineIndex.ToString() + ", " + currentIndex.ToString() + ": enemy type " + type.ToString() + " not found");
                }
            else Debug.LogError("line "+currentLineIndex.ToString()+", "+currentIndex.ToString()+": enemy type "+ type.ToString()+" not found");
            lastGen = 0;
            currentIndex++;
            if (currentIndex >= currentLine.Length) {
                RandomNewPattern();
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
