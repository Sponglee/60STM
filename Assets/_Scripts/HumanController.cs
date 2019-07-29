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
        exitRef = transform.parent.parent;
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
            AudioManager.Instance.PlaySound("Human");
            Destroy(gameObject);
        }
        else if(other.CompareTag("Human") && exitRef != other.GetComponent<HumanController>().exitRef)
        {
            if(Random.Range(1, 100)>60)
                StartCoroutine(StopShowMessage("!@#$%"));
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
        else if(navMeshPath.status == NavMeshPathStatus.PathPartial)
        {

            //Debug.Log("DESTO IS " + dest);
            dest = transform.parent.position;

            StopAllCoroutines();
            StartCoroutine(StopShowMessage("???"));
        }

        
        if (navMesh != null)
            navMesh.SetDestination(dest);
    }
   

    public IEnumerator StopShowMessage(string message, bool longerDuration = false)
    {
        humanCanvas.gameObject.SetActive(false);
        int probability = Random.Range(0, 100);
        //Debug.Log(probability);
        if(probability>80)
        {
            humanCanvas.gameObject.SetActive(true);
            humanCanvas.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = message;
            if(longerDuration)
            {
                yield return new WaitForSeconds(Random.Range(1.5f, 3f));
            }
            else
            {
                yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            }
            humanCanvas.gameObject.SetActive(false);

        }
       
    }

}
