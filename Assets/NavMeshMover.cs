using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshMover : MonoBehaviour
{

    private NavMeshAgent navMesh;
    private Vector3 dest;

    // Start is called before the first frame update
    void Start()
    {
        navMesh = GetComponent<NavMeshAgent>();
        
        dest = GameManager.Instance.rocket.position;
        navMesh.SetDestination(dest);

    }

    // Update is called once per frame
    void Update()
    {
      
    }
}
