using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OfficeController : MonoBehaviour
{
    [SerializeField]
    private Queue<Transform> humans;

    [SerializeField]
    private Transform exitPoint;

    [SerializeField]
    private float passRate = 1;



    private void Start()
    {

        humans = new Queue<Transform>();


        StartCoroutine(StopOffice());
    }


    public IEnumerator StopOffice()
    {
        while(true)
        {
            yield return new WaitForSeconds(passRate);
            if(humans.Count>0)
            {

                Transform tmpHuman = humans.Dequeue();

                tmpHuman.position = exitPoint.position;
                //tmpHuman.GetComponent<NavMeshMover>().Move();
            }
        }
    }

    public void AddQueue(Transform trans)
    {
        humans.Enqueue(trans);
    }

}
