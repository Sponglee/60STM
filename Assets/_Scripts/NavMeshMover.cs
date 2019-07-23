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
    }


    public void Move()
    {
        navMesh.speed = Random.Range(1f, 2f);
        navMesh.enabled = false;

        if (gameObject.CompareTag("ExtraRocket"))
        {
            dest = GameManager.Instance.rocketHolder.position;
        }
        else

        {
            dest = GameManager.Instance.rocketHolder.position;
        }
        
        navMesh.enabled = true;
        if(navMesh != null)
            navMesh.SetDestination(dest);
    }


}
