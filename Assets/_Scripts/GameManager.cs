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

    public Transform rocketHolder;
    public Transform previousRocket;

    public Transform debugTarget;

    public GameObject humanPref;


    public CinemachineVirtualCamera levelCam;

    //Total spawned people
    public int spawnCount = 1;
    //How many different spawns there is
    public int spawnModifier = 1;
    //Reference to count humans in rockets
    public int spawnHumanCount = 0;

    public bool SpawnedBool = false;

    [SerializeField]
    private float timer = 60;
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
            
            //Deselect transform
            selectedTile = null;
            //Enable Agent movement
            if(!selectedTileManager.RotationInProgress)
                selectedTileManager.AgentToggle(true);
            
        }
        else if(Input.GetMouseButtonDown(1))
        {
            LevelManager.Instance.DespawnExits();
            LevelManager.Instance.SpawnExit();
        }
        else if(Input.GetMouseButtonDown(2))
        {
            WinText.SetActive(true);
            timerText.gameObject.SetActive(false);
            //Win sequence
            levelCam.gameObject.SetActive(true);
            levelCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
            levelCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);
            rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");

        }

    }

    public IEnumerator LevelSpawner()
    {
        List<int> randomExits = new List<int>();

        for (int i = 0; i < 3; i++)
        {

            randomExits.Add(Random.Range(0, LevelManager.Instance.exits.Count));
        }

        yield return new WaitForSeconds(2f);


        while (Timer>0)
        {
            //Debug.Log(">>>>>>>>>>>" + levelGoal / LevelManager.Instance.freeTiles.Count);
            
            if(!SpawnedBool)
            {
                for (int i = 0; i < spawnModifier; i++)
                {
                    
                    LevelManager.Instance.SpawnExit();
                }
                //Set reference for goal count
                spawnHumanCount = humanCount;
                //Set reference to spawnCOunt
                spawnCount = 0;

                foreach (Transform exit in LevelManager.Instance.exits)
                {
                  
                    for (int i = 0; i < 15f; i++)
                    {
                        Instantiate(humanPref, exit.GetChild(2));
                        spawnCount += 1;
                    }
                }
                Debug.Log("SPAWN " + spawnCount);
                SpawnedBool = true;
                spawnModifier++;
            }
           

            yield return new WaitForSecondsRealtime(1f);
            Timer -= 1f;
           
            //Debug.Log(humanCount + "- " + spawnHumanCount + " ( " + spawnCount + ")");
           


           


            if (humanCount - spawnHumanCount >= spawnCount)
            {
                SpawnedBool = false;
                LevelManager.Instance.DespawnExits();
            }

            if (HumanCount >= levelGoal)
            {
                WinText.SetActive(true);
                timerText.gameObject.SetActive(false);
                //DIsable collider
                rocketHolder.GetChild(1).GetChild(0).GetComponent<BoxCollider>().enabled = false;
                //Win sequence

                WinText.SetActive(true);
                timerText.gameObject.SetActive(false);
                //Win sequence
                //levelCam.gameObject.SetActive(true);
                //levelCam.m_Follow = rocketHolder.GetChild(1).GetChild(1);
                //levelCam.m_LookAt = rocketHolder.GetChild(1).GetChild(1);
                rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");
                PlayerPrefs.SetInt("Level", PlayerPrefs.GetInt("Level", 1) + 1);

                yield break;
            }
        }

        //Reset the timer and spawn new rocket
        Timer = 60f;

        rocketHolder.GetChild(1).GetComponent<Animator>().SetTrigger("TakeOff");

        LevelManager.Instance.SpawnRocket();
        LevelManager.Instance.DisablePrevious(previousRocket);
        BuildSurface();
        yield return new WaitForSeconds(2f);

        yield return new WaitForSeconds(5f);
        LevelManager.Instance.RandomizePrevious(previousRocket);
        //LevelManager.Instance.DeletePrevious(previousRocket);
        randomExits.Clear();
        StartCoroutine(LevelSpawner());


    }

    //Rebake navmesh
    public void BuildSurface()
    {
        surface.BuildNavMesh();

        foreach (Transform item in LevelManager.Instance.transform)
        {
            if(item.childCount>0)
            {
                if(item.GetChild(0).CompareTag("Tile"))
                {
                    //Debug.Log(item.name + item.GetChild(0).name);
                    item.GetChild(0).GetComponent<TileManager>().AgentToggle(true);
                }
            }
        }
    }



    

}
