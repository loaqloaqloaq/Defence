using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 적 게임 오브젝트를 주기적으로 생성
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<Enemy> enemies = new List<Enemy>();  

    //ダメージ
    public float damageMax = 40f;
    public float damageMin = 20f;
    [SerializeField] Enemy[] enemyPrefabs;

    //体力
    public float healthMax = 200f;
    public float healthMin = 100f;

    //生成する位置
    public Transform[] spawnPoints;
    [SerializeField] private Transform enemyPool;　//enemy's parent object
    
    //移動速度
    public float speedMax = 3.5f;
    public float speedMin = 3f;

    //Waveパラメータ
    public Color strongEnemyColor = Color.red;
    public const float intensityModifier = 1.18f;
    private int wave;
    private int delay = 3;
    private bool stanby;

    private int enemyCount()
    {
        int count = 0;
        foreach (var enemy in enemies)
        {
            if (!enemy.dead)
            {
                ++count;
            } 
        }
        return count;
    }

    private void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameover) return;
        if (enemyCount() <= 0 && !stanby)
        {
            StartCoroutine(NextWave());
        }

        UpdateUI();
    }

    private void UpdateUI()
    {
        UIManager.Instance.UpdateWaveText(wave, enemyCount());
    }

    private void ClearEnemies()
    {
        if (enemies.Count <= 0)
            return;

        foreach (var enemy in enemies)
        {
            enemy.enabled = false;
        }
    }

    IEnumerator NextWave()
    {
        stanby = true;
        ClearEnemies();
        if (wave != 0)
        {
            UIManager.Instance.WaveClear();
            yield return new WaitForSeconds(delay);
        }
        StartWave();
        stanby = false;
    }

    public float GetWaveIntensity()
    {
        int powCount = wave - 1;
        return Mathf.Pow(intensityModifier, powCount);
    }

    private void StartWave()
    {
        ++wave;
        var spawnCount = Mathf.RoundToInt(wave * 6f); //生成する敵の数

        foreach (var obj in enemyPrefabs)  
        {
            if (obj == null)
            {
                return;
            }
        }

        for (int i = 1; i <= spawnCount; i++)
        {
            var enemyIntensity = Random.Range(0.1f, 1f); //敵の能力値を 10% ~ 100% ランダムで設定
            Enemy enemy;
            if (i <= enemies.Count)
            {
                int targetIndex = i - 1;
                enemy = enemies[targetIndex];
                RenewEnemy(enemyIntensity, enemy);
            }
            else
            {
                if (i % 10 == 0)
                {
                    enemy = enemyPrefabs[2];
                }
                else if (i % 5 == 0 || i % 4 == 0)
                {
                    enemy = enemyPrefabs[1];
                }
                else
                {
                    enemy = enemyPrefabs[0];
                }
                CreateEnemy(enemyIntensity, enemy);
            }
        }
    }

    //敵を生成
    private void CreateEnemy(float intensity, Enemy enemyPrefab) //敵
    {
        var enemy = Instantiate(enemyPrefab, enemyPool.transform);
        EnemySetup(intensity, enemy);
        enemies.Add(enemy);
        //enemy.OnDeath += () => enemies.Remove(enemy);
        //enemy.OnDeath += () => Destroy(enemy, 10f); //livingEntity에 내장되어 있는 OnDeath이벤트
        //enemy.OnDeath += () => GameManager.Instance.AddScore(enemy.currentScore); //
    }

    //シーンにすでにインスタンス化しているオブジェクトをWaveに合わせて更新
    private void RenewEnemy(float intensity, Enemy enemy) 
    {
        enemy.enabled = true;

        EnemySetup(intensity, enemy);
        enemy.Respawn();
    }

    private void EnemySetup(float intensity, Enemy enemy) //敵の能力値をWaveの合わせて設定
    {
        float waveIntensity = GetWaveIntensity(); 
        var health = Mathf.Lerp(healthMin, healthMax, intensity) * waveIntensity;
        var damage = Mathf.Lerp(damageMin, damageMax, intensity) * waveIntensity;
        var speed = Mathf.Lerp(speedMin, speedMax, intensity) * waveIntensity;
        var skinColor = Color.Lerp(Color.white, strongEnemyColor, intensity); //흰색 ~ 빨간색
        var animSpeed = Mathf.Lerp(1.0f, waveIntensity, intensity);
        enemy.Setup(health, damage, speed, speed * 0.3f, waveIntensity, animSpeed, skinColor); //speed * 0.3f는 패트롤 스피드
        
        var spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
        enemy.gameObject.transform.position = spawnPoint.position;
    }
}