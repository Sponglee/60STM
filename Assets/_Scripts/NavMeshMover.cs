using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMover : MonoBehaviour
{

    private NavMeshAgent navMesh;
    private Vector3 dest;
    private int roadIndex;
    public int RoadIndex { get => roadIndex; set => roadIndex = value; }


    // Start is called before the first frame update
    void Start()
    {
        
        navMesh = GetComponent<NavMeshAgent>();

        Move();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            GameManager.Instance.HumanCount++;
            Destroy(gameObject);
        }
        else if (other.CompareTag("Tile") /*|| other.CompareTag("Exit")*/)
        {
            transform.SetParent(other.transform.GetChild(4));
        }
    }


    public void Move()
    {

        navMesh.speed = Random.Range(1f, 2f);
        navMesh.enabled = false;





        navMesh.enabled = true;
        dest = CalculateNewPath();

       

        if(navMesh != null)
            navMesh.SetDestination(dest);
    }



    public Vector3 CalculateNewPath()
    {
        NavMeshPath path = new NavMeshPath();
        float tmpDistance = 99999f;
        Vector3 finalDest = Vector3.zero;

        foreach (Transform holder in GameManager.Instance.rocketHolders)
        {
            //Find closest available rocket
            dest = holder.position;
            navMesh.SetDestination(dest);
            navMesh.CalculatePath(dest, path);
            if (path.status == NavMeshPathStatus.PathPartial)
            {
                Debug.Log("SKIP");
                continue;
            }

            if (Vector3.Distance(dest,transform.position) < tmpDistance)
            {
                finalDest = dest;
                tmpDistance = Vector3.Distance(dest, transform.position);
            }
            Debug.Log("<<< DIST >>>> : " + Vector3.Distance(dest, transform.position) + " : " + tmpDistance);
        }

        return finalDest;
    }
}
