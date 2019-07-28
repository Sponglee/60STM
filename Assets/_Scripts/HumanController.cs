using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using TMPro;
public class HumanController : MonoBehaviour
{
    [SerializeField]
    private Vector3 dest;
    private int roadIndex;
    public int RoadIndex { get => roadIndex; set => roadIndex = value; }

    public Transform exitRef;
    public ExitManager exitManager;


    private NavMeshAgent navMesh;
    public NavMeshPath navMeshPath;

    public Canvas humanCanvas;

    // Start is called before the first frame update
    void Start()
    {
         //exitManager = exitRef.GetComponent<ExitManager>();
        navMesh = GetComponent<NavMeshAgent>();
        
        Move(GameManager.Instance.rocketHolder.position);

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


    public void Move(Vector3 toggleDesto)
    {
        navMeshPath = new NavMeshPath();

        navMesh.speed = Random.Range(3f, 4f);
      
        //navMesh.enabled = true;
        navMesh.CalculatePath(dest, navMeshPath);

        //Debug.Log("S: "+navMeshPath.status);
        if(navMeshPath.status == NavMeshPathStatus.PathComplete)
        {

            dest = toggleDesto;

            StopAllCoroutines();
            StartCoroutine(StopShowMessage(":)"));

        }
        else
        {

            //Debug.Log("DESTO IS " + dest);
            dest = transform.parent.position;

            StopAllCoroutines();
            StartCoroutine(StopShowMessage("???"));
        }

        
        if (navMesh != null)
            navMesh.SetDestination(dest);
    }
   

    public IEnumerator StopShowMessage(string message)
    {
        humanCanvas.gameObject.SetActive(false);
        int probability = Random.Range(0, 100);
        //Debug.Log(probability);
        if(probability>70)
        {
            humanCanvas.gameObject.SetActive(true);
            humanCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            humanCanvas.gameObject.SetActive(false);

        }
       
    }

}
