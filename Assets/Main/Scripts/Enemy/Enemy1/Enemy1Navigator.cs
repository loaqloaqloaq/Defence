using UnityEngine;

public class Enemy1Navigator : MonoBehaviour
{
    public Transform target,destination;    
    private Animator animator;
    private Enemy1Controller ec;
    private Transform g1, g2, g3;
    
    private Transform routes,area, route;
    private Vector3 checkPoint;
    private int checkPointIndex;
    private float offsetRange;


    // Start is called before the first frame update

    void Start()
    {
        animator = GetComponent<Animator>();
        ec = GetComponent<Enemy1Controller>();        
        g1 = GameObject.Find("Gate1").transform;
        g2 = GameObject.Find("Gate2").transform;
        g3 = GameObject.Find("Gate3").transform;

        routes = GameObject.Find("Routes").transform;

        destination = g1;

        offsetRange = 10f;
    }

    // Update is called once per frame
    void Update()
    {
        if (ec?.agent?.enabled == true && !ec.attacking)
        {
            target = ec.target ?? g1 ?? g2 ?? g3;            
            if (Vector3.Distance(transform.position, target.position) > 1.5f && !ec.attacking)
            {
                if ( (ec.gate == g2 && area != routes.GetChild(0)) || (ec.gate == g3 && area != routes.GetChild(1)) ) RandomRoute();
                CheckRoute();                

                animator.SetBool("walking", true);                
                Vector3 targetPos = Vector3.zero;
                if (target.CompareTag("Player")) targetPos = target.position;
                else targetPos = checkPoint;
                ec.agent.destination = targetPos;

            }
            else
            {
                animator.SetBool("walking", false);                
            }
        }
        else {
            animator.SetBool("walking", false);            
        }

        
    }   
    
    void NextCheckPoint()
    {
        checkPointIndex++;
        if (checkPointIndex >= route.childCount) {
            destination = ec.gate;
            checkPoint = destination.position;
            checkPoint.x += Random.Range(-1.5f, 1.5f); 
            return;
        }
        destination = route.GetChild(checkPointIndex);
        checkPoint = destination.position;
        checkPoint.x += Random.Range(-offsetRange, offsetRange);
        checkPoint.z += Random.Range(-offsetRange, offsetRange);
        checkPoint.y = 0;
    }
    void RandomRoute()
    {
        if(ec.gate == g2) area = routes.GetChild(0);
        else if(ec.gate == g3)area = routes.GetChild(1);
        route = area.GetChild(Random.Range(0, area.childCount));
        checkPointIndex = -1;
        NextCheckPoint();
    }

    void CheckRoute() {
        if (destination.CompareTag("Gate")) return;
        Vector3 pos = transform.position;
        pos.y = 0;
        if (Vector3.Distance(checkPoint, pos) < 0.2f) NextCheckPoint();


    }


}
