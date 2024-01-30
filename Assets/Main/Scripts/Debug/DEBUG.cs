using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class DEBUG : MonoBehaviour
{
    private IDamageable g1, g2, g3;
    DamageMessage gateDestory;

    EnemyBase[] eb;

    [SerializeField] bool escEnabled;

    [SerializeField] bool fireCannonEnabled;
    [SerializeField][Range(0, 10)] float timeScale;
    // Start is called before the first frame update
    void Start()
    {
        g1 = GameObject.Find("Gate1").GetComponent<IDamageable>();
        g2 = GameObject.Find("Gate2").GetComponent<IDamageable>();
        g3 = GameObject.Find("Gate3").GetComponent<IDamageable>();
        gateDestory = new DamageMessage();
        gateDestory.damager = gameObject;
        gateDestory.amount = 1000;

        eb=new EnemyBase[] { 
            GameObject.Find("enemyBase1")?.GetComponent<EnemyBase>()??null,
            GameObject.Find("enemyBase2")?.GetComponent<EnemyBase>()??null,
            GameObject.Find("enemyBase3")?.GetComponent<EnemyBase>()??null
        };

        foreach (EnemyBase b in eb) {
            if (b != null) 
                b.fireCannon = fireCannonEnabled;
        }

        timeScale = 1;

    }

    // Update is called once per frame
    void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.F10)) g1.ApplyDamage(gateDestory);
        if (Input.GetKeyDown(KeyCode.F11)) g2.ApplyDamage(gateDestory);
        if (Input.GetKeyDown(KeyCode.F12)) g3.ApplyDamage(gateDestory);

        if (Input.GetKey(KeyCode.Escape) && escEnabled)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
        }

        if (Input.GetKeyDown(KeyCode.F8)) {
            fireCannonEnabled = !fireCannonEnabled;
            foreach (EnemyBase b in eb)
            {
                b.fireCannon = fireCannonEnabled;
            }
        }

        if (Input.GetKeyDown(KeyCode.F7))
        {
            GameManager.Instance.AddScrap(9999);
        }
        if (Input.GetKeyDown(KeyCode.F6))
        {
            GameManager.Instance.DeductScrap(9999);
        }
        if (Input.GetKeyDown(KeyCode.F1))
        {
            timeScale = Time.timeScale;
            --timeScale;
            if (timeScale < 0) timeScale = 0;
            Time.timeScale = timeScale;
        }
        if (Input.GetKeyDown(KeyCode.F2))
        {
            timeScale = Time.timeScale;
            ++timeScale;
            if (timeScale > 5) timeScale = 5;
            Time.timeScale = timeScale;
        }
        if (Input.GetKeyDown(KeyCode.F3))
        {
            WarningController.ShowWarning("debug", "debug tesing test warning", 2);
        }




    }
}
