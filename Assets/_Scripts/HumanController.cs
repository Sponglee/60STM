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
    public Animator humanAnim;

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
            GameManager.Instance.RocketFilling++;
            GameManager.Instance.HumanCount++;
            //delete this human reference from exit tile
            exitManager.DespawnCheck();
            exitManager.humansRef.Remove(transform);
            AudioManager.Instance.PlaySound("Human");
            if (GameManager.Instance.zoomTargets.Contains(transform))
            {
                GameManager.Instance.zoomTargets.Remove(transform);
            }
            Destroy(gameObject);
        }
        else if (other.CompareTag("Human") && exitRef != other.GetComponent<HumanController>().exitRef)
        {
            if (Random.Range(1, 100) > 30)
                StartCoroutine(StopShowMessage("!@#$%"));
        }
        else if (other.CompareTag("Tile") || other.CompareTag("Exit"))
        {
            //Debug.Log(other.gameObject.name);
            transform.SetParent(other.transform.GetChild(4));
        }
        else if(other.CompareTag("Star"))
        {
            LevelManager.Instance.stars.Add(other.transform.parent.parent);
            Destroy(other.gameObject);
            FunctionHandler.Instance.UpdateStars(FunctionHandler.Instance.starsHolderUI);
           
        }
    }


    public void Move(Vector3 toggleDesto)
    {
        navMeshPath = new NavMeshPath();
        if(navMesh != null)
        {
            navMesh.speed = Random.Range(3f, 4f);

            //navMesh.enabled = true;
            navMesh.CalculatePath(toggleDesto, navMeshPath);
        }


        //Debug.Log("S: "+navMeshPath.status);
        if(navMeshPath.status == NavMeshPathStatus.PathComplete)
        {

            dest = toggleDesto;
           
            StopAllCoroutines();
            StartCoroutine(StopShowMessage(":)"));

            if (GameManager.Instance.zoomTargets.Count == 0) 
                AudioManager.Instance.PlaySound("Success");


            if(PlayerPrefs.GetInt("HoldTutShown",0) == 0)
            {
                FunctionHandler.Instance.HoldTut.SetActive(true);

                FunctionHandler.Instance.HoldTut.transform.parent.GetComponent<Animator>().SetTrigger("holdAnim");
                PlayerPrefs.SetInt("HoldTutShown", 1);
            }

            if(!GameManager.Instance.zoomTargets.Contains(transform))
            {
                GameManager.Instance.zoomTargets.Add(transform);
            }
        }
        else if(navMeshPath.status == NavMeshPathStatus.PathPartial)
        {

            //Debug.Log("DESTO IS " + dest);
            dest = transform.parent.position;

            StopAllCoroutines();
            StartCoroutine(StopShowMessage("???"));
        }

      
        StartCoroutine(StopDelayAnim());

        if (navMesh != null)
            navMesh.SetDestination(dest);
    }
   

    public IEnumerator StopDelayAnim()
    {
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        humanAnim.SetTrigger("Start");
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
