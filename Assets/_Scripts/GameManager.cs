using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Cinemachine;

public class GameManager : Singleton<GameManager>
{
   public Transform selectedTile;

    public NavMeshSurface surface;

    //Rocket to TakeOFF
    [SerializeField]
    private int rocketIndex = 0;
    public int RocketIndex
    {
        get
        {
            return rocketIndex;
        }
        set
        {
            if (value > rocketHolders.Count - 1)
                rocketIndex = 0;
            else if (value < 0)
                rocketIndex = rocketHolders.Count - 1;
            else
                rocketIndex = value;
        }
    }



    public List<Transform> rocketHolders;
    public Transform previousRocket;

    public Transform debugTarget;

    public GameObject humanPref;


    public CinemachineVirtualCamera levelCam;

    public int spawnCount = 1;


    public float spawnTimer = 0;
    public float spawnTime = 14f;
    [SerializeField]
    private float timer = 20;
    public float Timer
    {
        get
        {
            return timer;
        }
        set
        {
            timer = value;
            timerText.text = value.ToString();
        }
    }


    public GameObject WinText;

    public Text timerText;
    public Text humanText;

    public int levelGoal;
    [SerializeField]
    private int humanCount = 0;
    public int HumanCount
    {
        get
        {
            return humanCount;
        }
        set
        {
            humanCount = value;
            humanText.text = string.Format("{0}/{1}",value,levelGoal);
        }
    }

   

    private void Awake()
    {
        levelGoal = PlayerPrefs.GetInt("Level", 1)*50;
        rocketHolders = new List<Transform>();
    }

    private void Start()
    {

        humanText.text = string.Format("{0}/{1}", HumanCount, levelGoal);
        StartCoroutine(LevelSpawner());
    }


    private void Update()
    {
        if (Input.GetMouseButtonUp(0) && selectedTile != null)
        {
            TileManager selectedTileManager = selectedTile.GetComponent<TileManager>();
            selectedTileManager.CollidedBool = false;
            selectedTileManager.GetComponent<TileManager>().Selected = false;
            selectedTileManager.GetComponent<TileManager>().DragActive = false;
            
            ////Deselect transform
            //selectedTile = null;
            ////Enable Agent movement
            //if(!selectedTileManager.RotationInProgress)
            //    selectedTileManager.AgentToggle(true);
            
        }
        else if(Input.GetMouseButtonDown(1))
        {
            BuildSurface();

        }
        else if(Input.GetMouseButtonDown(2))
        {
            //WinText.SetActive(true);
            //timerText.gameObject.SetActive(false);
            ////Win sequence
            //levelCam.gameObject.SetActive(true);
            //levelCam.m_Follow = rocketHolders.GetChild(1).GetChild(1);
            //levelCam.m_LookAt = rocketHolders.GetChild(1).GetChild(1);
            //rocketHolders.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");

        }

    }

    public IEnumerator LevelSpawner()
    {
        List<int> randomExits = new List<int>();

        yield return new WaitForSeconds(0.5f);
        for (int i = 0; i < 3; i++)
        {

            randomExits.Add(Random.Range(0, LevelManager.Instance.exits.Count));
        }


        BuildSurface();

      

        while (Timer>0)
        {
            //Debug.Log(">>>>>>>>>>>" + levelGoal / LevelManager.Instance.freeTiles.Count);
            
            if(spawnCount<levelGoal && spawnTimer>=spawnTime)
            {
                spawnTimer = 0;
                for (int i = 0; i <Random.Range(0,5f); i++)
                {
                    Instantiate(humanPref, LevelManager.Instance.exits[randomExits[Random.Range(0, randomExits.Count)]].GetChild(2));
                }
                    spawnCount += 1;
            }


            spawnTimer += 1;
            yield return new WaitForSecondsRealtime(1);
            Timer -= 1f;

            //if (HumanCount >= levelGoal)
            //{
            //    WinText.SetActive(true);
            //    timerText.gameObject.SetActive(false);
            //    //DIsable collider
            //    rocketHolders.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
            //    //Win sequence

            //    WinText.SetActive(true);
            //    timerText.gameObject.SetActive(false);
            //    //Win sequence
            //    //levelCam.gameObject.SetActive(true);
            //    //levelCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
            //    //levelCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);
            //    rocketHolders.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
            //    PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

            //    yield break;
            //}
        }

       


        Timer = spawnTime;
        Debug.Log("RI" + RocketIndex);
        rocketHolders[RocketIndex].GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");


        if(previousRocket!=null)
        {
            previousRocket.GetChild(1).GetComponent<Animator>().SetTrigger("Return");
            previousRocket = rocketHolders[RocketIndex];
        }
        else
        {
            Debug.Log("Here");
            previousRocket = rocketHolders[RocketIndex]; 

        }

        //Stop Agents
        rocketHolders[RocketIndex].GetChild(0).gameObject.SetActive(false);
        BuildSurface();

       

        //Next rocket;
        RocketIndex++;

        

        StartCoroutine(LevelSpawner());
        //LevelManager.Instance.SpawnRocket();
        //LevelManager.Instance.DisablePrevious(previousRocket);
        yield return new WaitForSeconds(15f);

        previousRocket.GetChild(0).gameObject.SetActive(true);
        BuildSurface();
        yield return new WaitForSeconds(15f);

        //LevelManager.Instance.DeletePrevious(previousRocket);
        //randomExits.Clear();

        
        
    }

    //Rebake navmesh
    public void BuildSurface()
    {
        surface.BuildNavMesh();
        RecalculateRoute();
    }

    public void RecalculateRoute()
    {

        //recalculate route
        foreach (Transform child in LevelManager.Instance.transform)
        {
            if (child.GetChild(0).childCount > 4 && child.GetChild(0).GetChild(4).childCount > 0)
            {
                if(child.GetComponent<TileManager>() != null)
                    child.GetComponent<TileManager>().AgentToggle(true);

            }

        }


    }



}
