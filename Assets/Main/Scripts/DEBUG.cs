using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DEBUG : MonoBehaviour
{
    private IDamageable g1, g2, g3;
    DamageMessage gateDestory;
    // Start is called before the first frame update
    void Start()
    {
        g1 = GameObject.Find("Gate1").GetComponent<IDamageable>();
        g2 = GameObject.Find("Gate2").GetComponent<IDamageable>();
        g3 = GameObject.Find("Gate3").GetComponent<IDamageable>();
        gateDestory = new DamageMessage();
        gateDestory.damager = gameObject;
        gateDestory.amount = 1000;

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F10)) g1.ApplyDamage(gateDestory);
        if (Input.GetKeyDown(KeyCode.F11)) g2.ApplyDamage(gateDestory);
        if (Input.GetKeyDown(KeyCode.F12)) g3.ApplyDamage(gateDestory);
    }
}
