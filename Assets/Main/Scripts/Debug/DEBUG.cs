using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG : MonoBehaviour
{
    private IDamageable g1, g2, g3;
    DamageMessage gateDestory;

    EnemyBase[] eb;

    [SerializeField] bool escEnabled;

    [SerializeField] bool fireCannonEnabled;
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



    }
}
