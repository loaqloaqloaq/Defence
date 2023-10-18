using UnityEngine;
using UnityEngine.AI; //NavMesh 사용하기 위한 라이브러리 AI

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private Transform playerTransform;
    
    private float lastSpawnTime; //最後に生成した時間
    [SerializeField] private float maxDistance = 20f; //生成する位置からプレイヤーの位置までの最大距離 
    
    private float timeBetSpawn;
    [SerializeField] private float timeBetSpawnMax = 10f; //次のアイテムを生成するまでの時間
    [SerializeField] private float timeBetSpawnMin = 7f; // 時間はmin ~ max ランダムで決める

    private void Start()
    {
        timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        lastSpawnTime = 0f;
    }

    private void Update()
    {
        if (Time.time >= lastSpawnTime + timeBetSpawn && playerTransform != null)
        {
            Spawn();
            lastSpawnTime = Time.time;
            timeBetSpawn = Random.Range(timeBetSpawnMin, timeBetSpawnMax);
        }
    }

    private void Spawn()
    {
        var spawnPosition = Utility.GetRandomPointOnNavMesh(playerTransform.position, maxDistance,
            NavMesh.AllAreas); // 세번째 인자는 areakMask

        spawnPosition += Vector3.up * 0.5f;

        var item = Instantiate(items[Random.Range(0, items.Length)], spawnPosition, Quaternion.identity);
        Destroy(item, 30f);
    }
}