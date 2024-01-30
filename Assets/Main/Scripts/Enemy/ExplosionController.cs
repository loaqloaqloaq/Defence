using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionController : MonoBehaviour
{
    ParticleSystem ps;
    float timePassed;
    AudioSource audioClip;
    // Start is called before the first frame update
    void Start()
    {
        ps=GetComponent<ParticleSystem>();
        timePassed = 0;
        audioClip = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        audioClip.volume = transform.localScale.x/3;
        timePassed += Time.deltaTime;
        if(timePassed >= ps.main.duration) Destroy(gameObject);
    }
}
