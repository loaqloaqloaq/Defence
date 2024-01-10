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
    // Start is called before the first frame update
    void Start()
    {
        //�R���|�[�l���g��ǂݍ���
        npcController=GetComponent<FellowNPC>();
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
        agent.destination = checkpoint.position;
        //�e���|�[�g�G�t�F�N�g
        if (teleportEffect == null) teleportEffect = transform.Find("TeleportEffect").gameObject;
        pss = teleportEffect.GetComponentsInChildren<ParticleSystem>();       
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
        else {
            //�U����ړ��J�n
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

        //�`�F�b�N�|�C���g�̋߂��ɂ����玟�̃`�F�b�N�|�C���g
        if (DisToCheckPoint() < 0.5f) {
            NextCheckPoint();
        }
    }

    //�`�F�b�N�|�C���g�܂ł̋����v�Z
    float DisToCheckPoint() {
        var chkpt = checkpoint.position;
        var npcpt = transform.position;
        chkpt.y = npcpt.y = 0;
        return Vector3.Distance(chkpt, npcpt);
    }
    //���̃`�F�b�N�|�C���g���Z�b�g
    void NextCheckPoint() {
        currentPoint = (currentPoint + 1) % checkpoints.Length;
        checkpoint = checkpoints[currentPoint];
        agent.destination = checkpoint.position;
    }
    //���̃G���A�փe���|�[�g
    public void TeleportToNextRoute(int area) {
        if (area >= routes.childCount)
        {
            Debug.LogWarning($"route ${area} not exist");//���[�g������Ȃ���LOG�ɏ���
            return; //�e���|�[�g���~
        }
        agent.isStopped = true;
        agent.enabled = false;//�e���|�[�g���͈ړ����Ȃ�
        currentRoute = area;//���̃��[�g���Z�b�g        
        PlayEffect();//�G�t�F�N�g�Đ�
        Invoke("Teleport", 2.5f);
    }
    public void Teleport()
    {           
        //�`�F�b�N�|�C���g���X�g�X�V
        route = routes.GetChild(currentRoute);
        checkpoints = Array.FindAll(route.GetComponentsInChildren<Transform>(), child => child != route.transform);
        //�ړI�n���Z�b�g
        currentPoint = 0;
        checkpoint = checkpoints[currentPoint];       
        transform.position = checkpoint.position;
        agent.enabled = true;
        agent.isStopped = false;
    }

        //�e���|�[�g�G�t�F�N�g
    void PlayEffect()
    {
        foreach (ParticleSystem ps in pss)
        {
            ps.Play();
        }
    }
}
