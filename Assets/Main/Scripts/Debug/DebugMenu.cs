using System.Collections;
using System.Collections.Generic;
using System.Threading;
using TMPro;
using UnityEngine;

public class DebugMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI enemyGenTxt;
    [SerializeField] GameObject enemy;
    [SerializeField] GameObject enemyGuard;
    public void timer0()
    {
        GameManager.Instance.timer = 0;
    }
    public void life0()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().setLIfe(1);
        DamageMessage dm=new DamageMessage();
        dm.damager = gameObject;
        dm.amount = 9999;
        player.GetComponent<PlayerHealth>().ApplyDamage(dm);
    }
    public void lifeMax() {
        var player = GameObject.FindGameObjectWithTag("Player");
        player.GetComponent<PlayerController>().setLIfe(3);
        player.GetComponent<PlayerHealth>().maxHP();
    }
    public void enemyGen()
    {
        EnemyGeneratorManager.Instance.GenerateEnemy = !EnemyGeneratorManager.Instance.GenerateEnemy;
        enemyGenTxt.text = "敵のジェネレーター" + (EnemyGeneratorManager.Instance.GenerateEnemy ? "停止" : "開始");
    }
    public void GenEnemy(int type) {
        if (type >= 6) return;

        var player = GameObject.FindGameObjectWithTag("Player");
        var pool = GameObject.Find("Enemy Pool");
        if (pool == null) pool = new GameObject("Enemy Pool");
        GameObject e = Instantiate(enemy);        
        e.transform.SetParent(pool.transform, true);

        Vector3 pos = player.transform.position;
        pos += player.transform.forward * 3;
        e.gameObject.SetActive(true);
        e.transform.position = pos;
        e.GetComponent<EnemyController>().setType(type);            

    }
}
