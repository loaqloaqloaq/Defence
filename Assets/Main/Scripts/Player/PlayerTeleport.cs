using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTeleport : MonoBehaviour
{
    [SerializeField] GameObject teleportEffect;
    Vector3 targetPos;
    ParticleSystem[] pss;    
    // Start is called before the first frame update
    void Start()
    {
        if (teleportEffect == null) teleportEffect = transform.Find("TeleportEffect").gameObject;
        pss = teleportEffect.GetComponentsInChildren<ParticleSystem>();

        targetPos = Vector3.zero;        
    }

    public void TeleportTo(Vector3 pos) {
        if (targetPos == Vector3.zero && pos != Vector3.zero)
        {
            targetPos = pos;
            GetComponent<CharacterController>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            GetComponent<PlayerAiming>().enabled = false;
            GetComponent<PlayerLocomotion>().enabled = false;
            PlayEffect();
            Invoke("Teleport", 2.5f);
        }        

    }

    void Teleport() {
        if (targetPos != Vector3.zero) {                        
            transform.position = targetPos;
            GetComponent<CharacterController>().enabled = true;
            GetComponent<PlayerInput>().enabled = true;
            GetComponent<PlayerAiming>().enabled = true;
            GetComponent<PlayerLocomotion>().enabled = true;
            targetPos = Vector3.zero;           
        }
    }

    void PlayEffect() {
        foreach (ParticleSystem ps in pss)
        {
            ps.Play();
        }
    }
    void StopEffect() {
        foreach (ParticleSystem ps in pss)
        {
            ps.Stop();
        }
    }
}
