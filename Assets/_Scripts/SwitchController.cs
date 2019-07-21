using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SwitchController : MonoBehaviour
{

    private NavMeshObstacle obstacle;
    private Animator animator;

    private void Start()
    {
        obstacle = transform.GetComponent<NavMeshObstacle>();
        animator = transform.GetComponent<Animator>();
    }
    private void OnMouseDown()
    {
        obstacle.enabled = !obstacle.enabled;
        if(obstacle.enabled)
        {
            animator.SetTrigger("Close");
        }
        else
        {
            animator.SetTrigger("Open");
        }
        
    }
}
