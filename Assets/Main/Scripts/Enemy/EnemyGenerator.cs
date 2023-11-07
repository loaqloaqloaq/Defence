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
    int currentIndex;

    // Start is called before the first frame update
    void Start()
    {
        //Instantiate(enemy, transform.position, transform.rotation);        
        lastGen = 0;
        EnemyCnt = 0;
        
        string jsonString = PatternJsonFile.ToString();        
        patterns = JsonUtility.FromJson<Pattern>(jsonString).pattern;        

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
            Debug.Log(type);
            if (type >= 0 && type < enemy.Length)
                Instantiate(enemy[type], pos, transform.rotation);
            else Debug.LogError("enemy type not found");
            lastGen = 0;
            currentIndex++;
            if (currentIndex >= currentLine.Length) {
                RandomNewPattern();
            }
        }
    }

    private void RandomNewPattern()
    {
        currentLine = patterns[Random.Range(0, patterns.Length)].Split(",").Select(i => int.Parse(i)).ToArray();
        currentIndex = 0;
    }
}
