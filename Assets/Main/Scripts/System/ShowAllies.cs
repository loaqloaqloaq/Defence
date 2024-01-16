using UnityEngine;

public class ShowAllies : MonoBehaviour
{

    // Update is called once per frame
    void Update()
    {      

        var lookPos = Camera.main.transform.position - transform.position;
        lookPos.y = 0;
        transform.rotation = Quaternion.LookRotation(lookPos) * Quaternion.Euler(0, 180, 0);
    }
}
