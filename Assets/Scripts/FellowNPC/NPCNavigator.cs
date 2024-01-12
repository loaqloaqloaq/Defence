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
        //コンポーネントを読み込む
        npcController =GetComponent<FellowNPC>();
        agent =GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        //ルートを読み込む
        if (routes == null) routes = GameObject.Find("NPCRoutes").transform;
        currentRoute = 0;
        route = routes.GetChild(currentRoute);
        if (route !=null) checkpoints= Array.FindAll(route.GetComponentsInChildren<Transform>(), child => child != route.transform);
        //チェックポイントをセット
        currentPoint = 0;        
        checkpoint = checkpoints[currentPoint];
        agent.destination = checkpoint.GetComponent<Checkpoint>().GetPos();
        //テレポートエフェクト
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
            //攻撃中は移動しない            
            agent.isStopped = true;
        }
        //チェックポイントの近くについたら一時停止して次のチェックポイント
        else if (DisToCheckPoint() < 0.5f)
        {
            //一時停止
            agent.isStopped = true;
            stoppedTime += Time.deltaTime;
            if (stoppedTime >= stopTimer)
            {
                NextCheckPoint();
                ResetStopTimer();
            }
        }
        else {
            //移動開始
            agent.isStopped = false;
        }

        //移動アニメーシォン
        if (agent.isStopped) {
            animator.SetBool("Walking", false);            
            return;　//移動停止中は判定行わない
        }
        else
        {            
            animator.SetBool("Walking", true);           
        }

        
    }

    //チェックポイントまでの距離計算
    float DisToCheckPoint() {
        var chkpt = agent.destination;
        var npcpt = transform.position;
        chkpt.y = npcpt.y;
        return Vector3.Distance(chkpt, npcpt);
    }
    //次のチェックポイントをセット
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
    //エリアへテレポート
    public void TeleportToRoute(int area) {
        if (routes == null) routes = GameObject.Find("NPCRoutes").transform;
        if (area >= routes.childCount)
        {
            Debug.Log($"route {area} not exist");//ルート見つからない時LOGに書く
            return; //テレポート中止
        }
        Debug.Log($"NPC start teleporting to area {area}");
        //チェックポイントリスト更新
        currentRoute = area;//次のルートをセット        
        route = routes.GetChild(currentRoute);
        checkpoints = Array.FindAll(route.GetComponentsInChildren<Transform>(), child => child != route.transform);
        //目的地リセット
        currentPoint = 0;
        checkpoint = checkpoints[currentPoint];       
        //テレポート中は移動しない 
        agent.isStopped = true;
        agent.enabled = false;        
        
        PlayEffect();//エフェクト再生
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
    //テレポートエフェクト
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
