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
        if (other.CompareTag("Office"))
        {
            navMesh.SetDestination(other.transform.position);
            other.transform.parent.GetComponent<OfficeController>().AddQueue(transform);
        }
    }


    public void Move()
    {
        navMesh.enabled = false;
        dest = GameManager.Instance.rocket.position;
        navMesh.enabled = true;
        navMesh.SetDestination(dest);
    }


}
