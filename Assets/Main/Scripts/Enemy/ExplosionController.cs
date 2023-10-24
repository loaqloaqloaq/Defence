using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    ParticleSystem ps;
    float timePassed;
    // Start is called before the first frame update
    void Start()
    {
        ps=GetComponent<ParticleSystem>();
        timePassed = 0;
    }

    // Update is called once per frame
    void Update()
    {
        timePassed += Time.deltaTime;
        if(timePassed >= ps.main.duration) Destroy(gameObject);
    }
}
