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

    public Transform exitRef;
    public ExitManager exitManager;
    // Start is called before the first frame update
    void Start()
    {
         //exitManager = exitRef.GetComponent<ExitManager>();
        navMesh = GetComponent<NavMeshAgent>();

        Move();

    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Rocket"))
        {
            GameManager.Instance.HumanCount++;
            //delete this human reference from exit tile
            exitManager.DespawnCheck();
            exitManager.humansRef.Remove(transform);
            Destroy(gameObject);
        }
        else if (other.CompareTag("Tile") || other.CompareTag("Exit"))
        {
            transform.SetParent(other.transform.GetChild(4));
        }
    }


    public void Move()
    {
        navMesh.speed = Random.Range(3f, 4f);
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
