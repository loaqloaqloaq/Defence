using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCNavigator : MonoBehaviour
{
    [SerializeField] GameObject teleportEffect;
    [SerializeField] Transform routes;
    [SerializeField] Transform checkpoint;
    Transform route;
    Transform[] checkpoints;
    NavMeshAgent agent;    
    int currentRoute,currentPoint;
    FellowNPC npcController;
    Animator animator;
    ParticleSystem[] pss;

    float stopTimer, stoppedTime;
    // Start is called before the first frame update
    void Awake()
    {        
        //�R���|�[�l���g��ǂݍ���
        npcController =GetComponent<FellowNPC>();
        agent =GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //���[�g��ǂݍ���
        if (routes == null) routes = GameObject.Find("NPCRoutes").transform;
        currentRoute = 0;
        route = routes.GetChild(currentRoute);
        if (route !=null) checkpoints= Array.FindAll(route.GetComponentsInChildren<Transform>(), child => child != route.transform);
        //�`�F�b�N�|�C���g���Z�b�g
        currentPoint = 0;        
        checkpoint = checkpoints[currentPoint];
        agent.destination = checkpoint.GetComponent<Checkpoint>().GetPos();
        //�e���|�[�g�G�t�F�N�g
        if (teleportEffect == null) teleportEffect = transform.Find("TeleportEffect").gameObject;
        pss = teleportEffect.GetComponentsInChildren<ParticleSystem>();

        ResetStopTimer();
    }

    // Update is called once per frame
    void Update()
    {
        if (!agent.enabled) return;
        if (npcController.fellowAI == FellowNPC.State.Attack)
        {
            //�U�����͈ړ����Ȃ�            
            agent.isStopped = true;
        }
        //�`�F�b�N�|�C���g�̋߂��ɂ�����ꎞ��~���Ď��̃`�F�b�N�|�C���g
        else if (DisToCheckPoint() < 0.5f)
        {
            //�ꎞ��~
            agent.isStopped = true;
            stoppedTime += Time.deltaTime;
            if (stoppedTime >= stopTimer)
            {
                NextCheckPoint();
                ResetStopTimer();
            }
        }
        else {
            //�ړ��J�n
            agent.isStopped = false;
        }

        //�ړ��A�j���[�V�H��
        if (agent.isStopped) {
            animator.SetBool("Walking", false);            
            return;�@//�ړ���~���͔���s��Ȃ�
        }
        else
        {            
            animator.SetBool("Walking", true);           
        }

        
    }

    //�`�F�b�N�|�C���g�܂ł̋����v�Z
    float DisToCheckPoint() {
        var chkpt = agent.destination;
        var npcpt = transform.position;
        chkpt.y = npcpt.y;
        return Vector3.Distance(chkpt, npcpt);
    }
    //���̃`�F�b�N�|�C���g���Z�b�g
    void NextCheckPoint() {
        currentPoint = (currentPoint + 1) % checkpoints.Length;
        checkpoint = checkpoints[currentPoint];
        var pos = checkpoint.GetComponent<Checkpoint>().GetPos();
        float range = 2f;
        Vector3 rand = new Vector3(ran(range), 0, ran(range));
        agent.destination = pos + rand;
    }
    float ran(float range) { 
        return UnityEngine.Random.Range(-range,range);
    }
    //�G���A�փe���|�[�g
    public void TeleportToRoute(int area) {
        if (routes == null) routes = GameObject.Find("NPCRoutes").transform;
        if (area >= routes.childCount)
        {
            Debug.Log($"route {area} not exist");//���[�g������Ȃ���LOG�ɏ���
            return; //�e���|�[�g���~
        }
        Debug.Log($"NPC start teleporting to area {area}");
        //�`�F�b�N�|�C���g���X�g�X�V
        currentRoute = area;//���̃��[�g���Z�b�g        
        route = routes.GetChild(currentRoute);
        checkpoints = Array.FindAll(route.GetComponentsInChildren<Transform>(), child => child != route.transform);
        //�ړI�n���Z�b�g
        currentPoint = 0;
        checkpoint = checkpoints[currentPoint];       
        //�e���|�[�g���͈ړ����Ȃ� 
        agent.isStopped = true;
        agent.enabled = false;        
        
        PlayEffect();//�G�t�F�N�g�Đ�
        Invoke("Teleport", 2.5f);
    }    
    public void Teleport()
    {          
        transform.position = checkpoint.GetComponent<Checkpoint>().GetPos();
        if (agent)
        {
            agent.enabled = true;
            agent.isStopped = false;
        }
    }
    //�e���|�[�g�G�t�F�N�g
    void PlayEffect()
    {
        foreach (ParticleSystem ps in pss)
        {
            ps.Play();
        }
    }
    void ResetStopTimer() {
        stopTimer = 1.5f + ran(0.5f);
        stoppedTime = 0;
    }
}
